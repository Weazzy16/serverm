export const secondsToTime = (waitSeconds) => {
    if (waitSeconds <= 0) 
        return `0 сек.`

    const hours = Math.floor(waitSeconds / 3600);
    const minutes = Math.floor((waitSeconds % 3600) / 60);
    const seconds = Math.floor(waitSeconds % 60);

    let text = "";
    if (hours > 0) text += `${hours} ч. `;
    if (minutes > 0) text += `${minutes} мин. `;
    if (seconds > 0) text += `${seconds} сек. `;

    return text;
};
