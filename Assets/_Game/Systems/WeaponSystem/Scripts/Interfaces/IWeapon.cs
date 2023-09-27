public interface IWeapon
{
	public WeaponData WeaponData { get; set; }
	
	void Attack();
	void Equip();
	void Unequip();
}
