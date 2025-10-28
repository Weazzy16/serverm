export const CREW_ACCESS = {
    MEMBER: 0,
    COMMANDER: 1,
    LEADER: 2
}

/**
 * Длительность показа маркера
 */
export const ALERT_MARKER_DURATION = 7000;

/**
 * Максимальная дистанция отображения маркера
 */
export const ALERT_MARKER_DISTANCE = 250;

/**
 * RGBA Цвета маркеров в зависимоти от ранга
 */
export const ALERT_MARKER_COLORS = {
    [CREW_ACCESS.MEMBER]: [14, 98, 13, 255],
    [CREW_ACCESS.COMMANDER]: [172, 125, 21, 255],
    [CREW_ACCESS.LEADER]: [182, 162, 26, 255]
};

/**
 * Цвета игроков в зависимости от ранга
 * Все цвета тут - https://wiki.rage.mp/wiki/Blips
 */
export const PLAYER_BLIPS_COLORS = {
    [CREW_ACCESS.MEMBER]: 38,
    [CREW_ACCESS.COMMANDER]: 28,
    [CREW_ACCESS.LEADER]: 25
};

export const CREW_VARIABLE_NAME = "CREW:MEMBER";