import { isPng, getPng } from "@/views/player/menu/getPng";

// export const daysName = [
//     "Понедельник",
//     "Вторник",
//     "Среда",
//     "Четверг",
//     "Пятница",
//     "Суббота",
//     "Воскресенье",
//     "Еженедельная награда",
// ]

export const daysName = [
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday",
    "Sunday",
    "Weekly reward",
]

const getPngToItemId = (item) => {
    const itemData = {
        ItemId: item.itemId,
        Data: item.data,
    }
    return getPng (itemData, window.getItem (item.itemId))
}

export const getPngUrl = (item) => {
    if (isPng(item.image))
        return item.image;
    else
        return getPngToItemId (item);
}

export const getTypeToTitle = (type, itemId) => {
    switch (type)
    {
        case 0:
            return window.getItem (itemId).Name;
        case 1:
            switch (itemId)
            {
                case 1:
                    return "VIP Silver";
                case 2:
                    return "VIP Gold";
                case 3:
                    return "VIP Platinum";
                case 4:
                    return "VIP Diamond";
            }
            break;
        case 2:
            return "Game currency";
        case 3:
            return "RedBucks";
    }
    return "";
}

export const statusType = {
    none: 0,
    received: 1,
    receive: 2,
    skip: 3,
    time: 4,
}

const formatIntZero = (num, length) => {
    return ("0" + num).slice(0 - length);
}

export const getTimeFromMins = (mins, isNonSec = false) => {
    let hours = Math.trunc(mins / 60);
    let minutes = mins % 60;
    if (isNonSec)
        return formatIntZero(hours, 2) + ':' + formatIntZero(minutes, 2);
        
    return formatIntZero(hours, 2) + ':' + formatIntZero(minutes, 2) + ':00';
}

export const getTimeFromMinsText = (mins) => {
    let hours = Math.trunc(mins / 60);
    let minutes = mins % 60;

    let minutesText = "minutes";

    switch(minutes) {
        case 1:
            minutesText = "minute";
            break;
        case 2:
        case 3:
        case 4:
            minutesText = "minutes";
            break;
    }

    if (hours > 0) {
        let hoursText = "hours";
        switch(hours) {
            case 1:
                hoursText = "hour";
                break;
            case 2:
            case 3:
            case 4:
                hoursText = "hour's";
                break;
        }

        return `${hours} ${hoursText} ${minutes} ${minutesText}`;
    }

    return `${minutes} ${minutesText}`;
}