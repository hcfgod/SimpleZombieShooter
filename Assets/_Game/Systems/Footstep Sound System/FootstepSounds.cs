using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;

public class FootstepSounds : MonoBehaviour
{
    #region VariablesAndMethods

	[SerializeField] private NormalMovement _playerMovment;
	[SerializeField] private PlayerData _playerData;
	
	[SerializeField] private LayerMask FloorLayer;
	[SerializeField] private TextureSound[] TextureSounds;
	[SerializeField] private bool BlendTerrainSounds;

	[SerializeField] private List<AudioClip> defaultFootstepSounds;
	[SerializeField] private List<AudioClip> defaultLandingSounds;
	
	private bool _canPlayLandedSound = false;

	private Coroutine checkGroundRoutine;
	private Coroutine footstepRoutine;
	
	private bool isFootstepCoroutineRunning = false;
	
    #endregion

    #region MonoBehaviour
	
	private void Start()
	{
		_playerMovment.CharacterActor.OnLanded += Landed;
		
		if(checkGroundRoutine == null && footstepRoutine == null)
			checkGroundRoutine = StartCoroutine(CheckGround());
	}

	private void OnEnable()
	{	
		checkGroundRoutine = StartCoroutine(CheckGround());		
	}
	
	private void OnDisable()
	{
		CleanupCoroutines();
	}
	
	private void OnDestroy()
	{
		_playerMovment.CharacterActor.OnLanded -= Landed;
		
		CleanupCoroutines();
	}
	
    #endregion

    #region Methods
	
	#region Util Methods
	
	private IEnumerator CheckGround()
	{
		while (true)
		{
			if (_playerData.isGrounded)
			{
				RaycastHit hit;
				if (IsPlayerOnGround(out hit))
				{
					HandleGroundedState(hit);
				}
			}
			yield return null;
		}
	}
	
	private bool IsPlayerOnGround(out RaycastHit hit)
	{
		return Physics.Raycast(transform.position, Vector3.down, out hit, 1f, FloorLayer);
	}
	
	private void CleanupCoroutines()
	{
		if(checkGroundRoutine != null && footstepRoutine != null)
		{
			StopCoroutine(checkGroundRoutine);
			StopCoroutine(footstepRoutine);
			
			checkGroundRoutine = null;
			footstepRoutine = null;
		}
	}
	
	#endregion
	
	#region Handle Methods
	
	private void HandleGroundedState(RaycastHit hit)
	{
		if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
		{
			HandleTerrain(terrain, hit.point);
		}
		else if (hit.collider.TryGetComponent<Renderer>(out Renderer renderer))
		{
			HandleRenderer(renderer);
		}
		else
		{
			HandleOtherCases(hit);
		}
	}
	
	private void HandleTerrain(Terrain terrain, Vector3 hitPoint)
	{
		if (_canPlayLandedSound)
		{
			PlayLandedSoundFromTerrain(terrain, hitPoint);
			_canPlayLandedSound = false;
		}
		
		if (!isFootstepCoroutineRunning)
		{
			footstepRoutine = StartCoroutine(PlayFootstepSoundFromTerrain(terrain, hitPoint));
		}
	}

	private void HandleRenderer(Renderer renderer)
	{
		if (_canPlayLandedSound)
		{
			PlayLandedSoundFromRenderer(renderer);
			_canPlayLandedSound = false;
		}
		
		if (!isFootstepCoroutineRunning)
		{
			footstepRoutine = StartCoroutine(PlayFootstepSoundFromRenderer(renderer));
		}
	}
	
	private void HandleOtherCases(RaycastHit hit)
	{
		// Try to get the component in children
		if (hit.collider.GetComponentInChildren<Terrain>())
		{
			Terrain childTerrain = hit.collider.GetComponentInChildren<Terrain>();

			if (_canPlayLandedSound)
			{
				PlayLandedSoundFromTerrain(childTerrain, hit.point);
				_canPlayLandedSound = false;
			}

			if(!isFootstepCoroutineRunning)
			{
				footstepRoutine = StartCoroutine(PlayFootstepSoundFromTerrain(childTerrain, hit.point));
			}
		}
		else if (hit.collider.GetComponentInChildren<Renderer>())
		{
			Renderer childRenderer = hit.collider.GetComponentInChildren<Renderer>();

			if (_canPlayLandedSound)
			{
				PlayLandedSoundFromRenderer(childRenderer);
				_canPlayLandedSound = false;
			}

			if(!isFootstepCoroutineRunning)
			{
				footstepRoutine = StartCoroutine(PlayFootstepSoundFromRenderer(childRenderer));
			}
		}
	}
	
	#endregion
	
	#region PlaySound Methods
	
	private void PlayLandedSoundFromTerrain(Terrain Terrain, Vector3 HitPoint)
	{
		bool soundPlayed = false;
		
		Vector3 terrainPosition = HitPoint - Terrain.transform.position;
		Vector3 splatMapPosition = new Vector3(terrainPosition.x / Terrain.terrainData.size.x, 0, terrainPosition.z / Terrain.terrainData.size.z);

		int x = Mathf.FloorToInt(splatMapPosition.x * Terrain.terrainData.alphamapWidth);
		int z = Mathf.FloorToInt(splatMapPosition.z * Terrain.terrainData.alphamapHeight);

		float[,,] alphaMap = Terrain.terrainData.GetAlphamaps(x, z, 1, 1);

		if (!BlendTerrainSounds)
		{
			int primaryIndex = 0;

			for (int i = 1; i < alphaMap.Length; i++)
			{
				if (alphaMap[0, 0, i] > alphaMap[0, 0, primaryIndex])
				{
					primaryIndex = i;
				}
			}

			foreach (TextureSound textureSound in TextureSounds)
			{
				if (textureSound.Albedo == Terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture)
				{
					AudioClip clip = GetLandedAudioFromTextureSound(textureSound);
					AudioManager.instance.PlaySFX(clip, textureSound.landingVolume, true);
					
					soundPlayed = true;
				}
			}
		}
		else
		{
			List<AudioClip> clips = new List<AudioClip>();

			int clipIndex = 0;

			for (int i = 0; i < alphaMap.Length; i++)
			{
				if (alphaMap[0, 0, i] > 0)
				{
					foreach (TextureSound textureSound in TextureSounds)
					{
						if (textureSound.Albedo == Terrain.terrainData.terrainLayers[i].diffuseTexture)
						{
							AudioClip clip = GetLandedAudioFromTextureSound(textureSound);
							AudioManager.instance.PlaySFX(clip, textureSound.landingVolume, true);

							clips.Add(clip);
							clipIndex++;
							
							soundPlayed = true;
						}
					}
				}
			}
		}
		
		if (!soundPlayed)
		{
			AudioClip randomClip = defaultLandingSounds[Random.Range(0, defaultLandingSounds.Count)];
			AudioManager.instance.PlaySFX(randomClip, 0.025f, true);
		}
	}

	private void PlayLandedSoundFromRenderer(Renderer Renderer)
	{
		bool soundPlayed = false;
		
		foreach (TextureSound textureSound in TextureSounds)
		{
			if (textureSound.Albedo == Renderer.material.GetTexture("_MainTex"))
			{
				AudioClip clip = GetLandedAudioFromTextureSound(textureSound);
				AudioManager.instance.PlaySFX(clip, textureSound.sprintingVolume, true);
				soundPlayed = true;
				break;
			}
		}

		if (!soundPlayed)
		{
			AudioClip randomClip = defaultLandingSounds[Random.Range(0, defaultLandingSounds.Count)];
			AudioManager.instance.PlaySFX(randomClip, 0.025f, true);
		}
	}
	
	private IEnumerator PlayFootstepSoundFromTerrain(Terrain Terrain, Vector3 HitPoint)
	{
		isFootstepCoroutineRunning = true;
		bool soundPlayed = false;
		
		Vector3 terrainPosition = HitPoint - Terrain.transform.position;
		Vector3 splatMapPosition = new Vector3(terrainPosition.x / Terrain.terrainData.size.x,0,terrainPosition.z / Terrain.terrainData.size.z);

		int x = Mathf.FloorToInt(splatMapPosition.x * Terrain.terrainData.alphamapWidth);
		int z = Mathf.FloorToInt(splatMapPosition.z * Terrain.terrainData.alphamapHeight);

		float[,,] alphaMap = Terrain.terrainData.GetAlphamaps(x, z, 1, 1);

		if (!BlendTerrainSounds)
		{
			int primaryIndex = 0;

			for (int i = 1; i < alphaMap.Length; i++)
			{
				if (alphaMap[0, 0, i] > alphaMap[0, 0, primaryIndex])
				{
					primaryIndex = i;
				}
			}

			foreach (TextureSound textureSound in TextureSounds)
			{
				if (textureSound.Albedo == Terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture)
				{
					AudioClip clip = GetFootstepAudioFromTextureSound(textureSound);

					if (_playerData.isRunning)
					{
						AudioManager.instance.PlaySFX(clip, textureSound.sprintingVolume, true);

						yield return new WaitForSeconds(textureSound.sprintingDelay);
					}
					else if (_playerData.isWalking && !_playerData.isCrouching)
					{
						AudioManager.instance.PlaySFX(clip, textureSound.walkingVolume, true);

						yield return new WaitForSeconds(textureSound.walkingDelay);
					}
					else if (_playerData.isCrouching && _playerData.isWalking)
					{
						AudioManager.instance.PlaySFX(clip, textureSound.crouchingVolume, true);

						yield return new WaitForSeconds(textureSound.crouchingDelay);
					}

					soundPlayed = true;
					
					break;
				}
			}
		}
		else
		{
			List<AudioClip> clips = new List<AudioClip>();

			int clipIndex = 0;

			for (int i = 0; i < alphaMap.Length; i++)
			{
				if (alphaMap[0, 0, i] > 0)
				{
					foreach(TextureSound textureSound in TextureSounds)
					{
						if (textureSound.Albedo == Terrain.terrainData.terrainLayers[i].diffuseTexture)
						{
							AudioClip clip = GetFootstepAudioFromTextureSound(textureSound);
                            
							if (_playerData.isRunning)
							{
								yield return new WaitForSeconds(textureSound.sprintingDelay);
								AudioManager.instance.PlaySFX(clip, textureSound.sprintingVolume, true);
							}
							else if (_playerData.isWalking && !_playerData.isCrouching)
							{
								yield return new WaitForSeconds(textureSound.walkingVolume);
								AudioManager.instance.PlaySFX(clip, textureSound.walkingVolume, true);
							}
							else if (_playerData.isCrouching && _playerData.isWalking)
							{
								yield return new WaitForSeconds(textureSound.crouchingVolume);
								AudioManager.instance.PlaySFX(clip, textureSound.crouchingVolume, true);
							}

							clips.Add(clip);
							clipIndex++;
							
							soundPlayed = true;
							
							break;
						}
					}
				}
			}
		}	
		
		if (!soundPlayed)
		{
			AudioClip randomClip = defaultFootstepSounds[Random.Range(0, defaultFootstepSounds.Count)];

			if (_playerData.isRunning)
			{
				AudioManager.instance.PlaySFX(randomClip, 0.05f, true);
				yield return new WaitForSeconds(0.3f);
			}
			else if (_playerData.isWalking && !_playerData.isCrouching)
			{
				AudioManager.instance.PlaySFX(randomClip, 0.025f, true);
				yield return new WaitForSeconds(0.55f);
			}
			else if (_playerData.isCrouching && _playerData.isWalking)
			{
				AudioManager.instance.PlaySFX(randomClip, 0.01f, true);
				yield return new WaitForSeconds(0.75f);
			}
		}

		isFootstepCoroutineRunning = false;
	}

	private IEnumerator PlayFootstepSoundFromRenderer(Renderer Renderer)
	{
		bool soundPlayed = false;

		isFootstepCoroutineRunning = true;
		
		foreach (TextureSound textureSound in TextureSounds)
		{
			if (textureSound.Albedo == Renderer.material.GetTexture("_MainTex"))
			{
				AudioClip clip = GetFootstepAudioFromTextureSound(textureSound);
				
				if (_playerData.isRunning)
				{
					AudioManager.instance.PlaySFX(clip, textureSound.sprintingVolume, true);
					yield return new WaitForSeconds(textureSound.sprintingDelay);
				}
				else if (_playerData.isWalking && !_playerData.isCrouching)
				{
					AudioManager.instance.PlaySFX(clip, textureSound.walkingVolume, true);
					yield return new WaitForSeconds(textureSound.walkingDelay);
				}
				else if (_playerData.isCrouching && _playerData.isWalking)
				{
					AudioManager.instance.PlaySFX(clip, textureSound.crouchingVolume, true);
					yield return new WaitForSeconds(textureSound.crouchingDelay);
				}
				
				soundPlayed = true;
				
				break;
			}
		}

		if (!soundPlayed)
		{
			AudioClip randomClip = defaultFootstepSounds[Random.Range(0, defaultFootstepSounds.Count)];

			if (_playerData.isRunning)
			{
				AudioManager.instance.PlaySFX(randomClip, 0.05f, true);
				yield return new WaitForSeconds(0.3f);
			}
			else if (_playerData.isWalking && !_playerData.isCrouching)
			{
				AudioManager.instance.PlaySFX(randomClip, 0.025f, true);
				yield return new WaitForSeconds(0.55f);
			}
			else if (_playerData.isCrouching && _playerData.isWalking)
			{
				AudioManager.instance.PlaySFX(randomClip, 0.01f, true);
				yield return new WaitForSeconds(0.75f);
			}
		}

		isFootstepCoroutineRunning = false;
	}

	private AudioClip GetFootstepAudioFromTextureSound(TextureSound TextureSound)
	{
		int clipIndex = Random.Range(0, TextureSound.footstepAudio.Length);
		return TextureSound.footstepAudio[clipIndex];
	}

	private AudioClip GetLandedAudioFromTextureSound(TextureSound TextureSound)
	{
		int clipIndex = Random.Range(0, TextureSound.landingAudio.Length);
		return TextureSound.landingAudio[clipIndex];
	}
	
	#endregion
		
	private void Landed()
	{
		_canPlayLandedSound = true;
	}

    #endregion
}
