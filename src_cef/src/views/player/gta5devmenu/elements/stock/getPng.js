import { ItemType, ItemId } from 'json/itemsInfo.js';

// Конвертер текста в boolean
const Bool = (text) => String(text).toLowerCase() === 'true';

/**
 * getPngToItemId — вспомогательная функция,
 * возвращает объект { name, png } по данным из колеса
 */
export const getPngToItemId = (data) => {
  let title = data.Name || '';
  let png = data.Png || '';

  switch (data.Type) {
    case 0: {
      // Обычный предмет
      const itemInfo = window.getItem?.(data.ItemId);
      png = getPng(data, itemInfo);
      if (!title) {
        title = data.ItemId === ItemId.CarCoupon
          ? (data.Data || '').toLowerCase()
          : itemInfo?.Name || '';
      }
      break;
    }
    case 1: // VIP
      switch (data.ItemId) {
        case 1:
          png = document.cloud + 'img/roulette/items_13.png';
          title = title || 'VIP Silver';
          break;
        case 2:
          png = document.cloud + 'img/roulette/items_14.png';
          title = title || 'VIP Gold';
          break;
        case 3:
          png = document.cloud + 'img/roulette/items_15.png';
          title = title || 'VIP Platinum';
          break;
        case 4:
          png = document.cloud + 'img/roulette/items_16.png';
          title = title || 'VIP Diamond';
          break;
      }
      break;
    case 2:
      png = document.cloud + 'img/roulette/items_0.png';
      title = title || 'Game currency';
      break;
    case 3:
      png = document.cloud + 'img/roulette/items_5.png';
      title = title || 'RedBucks';
      break;
  }

  return { name: title, png };
};

/**
 * isPng — проверка строки на URL
 */
export const isPng = (url) => {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
};

/**
 * getPng — возвращает URL картинки по localItem и данным iconInfo
 */
export const getPng = (localItem, iconInfo) => {
  try {
    // Валидация localItem
    if (!localItem || typeof localItem.ItemId !== 'number') {
      console.warn('getPng: некорректный элемент', localItem);
      return '';
    }

    const id = localItem.ItemId;
    const data = String(localItem.Data || '');
    const parts = data.split('_');

    // Если нет iconInfo или нет поля functionType — фоллбек
    if (!iconInfo || typeof iconInfo.functionType !== 'number') {
      console.warn(`getPng: нет данных iconInfo.functionType для ItemId=${id}`);
      return `http://cdn.piecerp.ru/cloud/` + `inventoryItems/items/${id}.png`;
    }

    // Одежда (кроме бронежилета)
    if (id !== ItemId.BodyArmor && iconInfo.functionType === ItemType.Clothes) {
      let dir = 'inventoryItems/clothes';
      const drawable = Number(parts[0] || 0);
      const texture = Number(parts[1] || 0);
      const male = Bool(parts[2]);

      dir += male ? '/male' : '/female';

      // Поддиректории для разных частей одежды
      switch (id) {
        case ItemId.Mask:      dir += '/masks';      break;
        case ItemId.Glasses:   dir += '/glasses';   break;
        case ItemId.Ears:      dir += '/ears';      break;
        case ItemId.Jewelry:   dir += '/accessories';break;
        case ItemId.Bracelets: dir += '/bracelets'; break;
        case ItemId.Hat:       dir += '/hats';      break;
        case ItemId.Leg:       dir += '/legs';      break;
        case ItemId.Feet:      dir += '/shoes';     break;
        case ItemId.Top:       dir += '/tops';      break;
        case ItemId.Undershit: dir += '/undershit'; break;
        case ItemId.Watches:   dir += '/watches';   break;
        case ItemId.Bag:       dir += '/bags';      break;
        case ItemId.Gloves:    dir += '/gloves';    break;
      }

      return `http://cdn.piecerp.ru/cloud/` + `${dir}/${drawable}_${texture}.png`;
    }

    // Купон на машину
    if (id === ItemId.CarCoupon) {
      return `http://cdn.piecerp.ru/cloud/` + `inventoryItems/vehicle/${data.toLowerCase()}.png`;
    }

    // По умолчанию — обычные предметы
    return `http://cdn.piecerp.ru/cloud/` + `inventoryItems/items/${id}.png`;
  }
  catch (err) {
    console.error('getPng error:', err);
    return '';
  }
};
