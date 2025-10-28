using NeptuneEvo.Core.Armor.Classes;
using System;
using System.Collections.Generic;
using System.Text;

     static class WeaponConfig
{
    public static readonly Dictionary<int, WeaponData> WEAPON_SKIN_TYPES = new Dictionary<int, WeaponData> {

    // ���� � ��� ��� internal ID �����
    { 260, new WeaponData(weaponId:260, name:"Classic Rifle Skin", modelHash:0xABC123 /*����� ������� prefab/hash*/, category:"��������") },
    { 263, new WeaponData(weaponId:263, name:"Golden Pistol Skin", modelHash:0xDEF456, category:"���������") },
    // � � �.�.
  };

    public static WeaponData? GetSkin(int id)
      => WEAPON_SKIN_TYPES.TryGetValue(id, out var d) ? d : null;
}

public class WeaponData
{
    public int WeaponId { get; }
    public string Name { get; }
    public uint ModelHash { get; }
    public string Category { get; }

    public WeaponData(int weaponId, string name, uint modelHash, string category)
    {
        WeaponId = weaponId;
        Name = name;
        ModelHash = modelHash;
        Category = category;
    }
}
