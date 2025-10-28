import { writable } from 'svelte/store';

export const coins = writable("200");
export const time = writable("05:00");
export const received = writable(0);
export const opened = writable(false);

export const open = () => opened.set(true);
export const close = () => {
    opened.set(false);
    clearInterval(countdownInterval);
};

let countdownInterval;

export function startCountdown(initialTime) {
    if (countdownInterval) clearInterval(countdownInterval);

    let [hours, minutes] = initialTime.split(":").map(Number);
    let totalSeconds = (hours * 3600) + (minutes * 60);

    countdownInterval = setInterval(() => {
        if (totalSeconds <= 0) {
            clearInterval(countdownInterval);
            countdownInterval = null;
            time.set("00:00");
            received.set(1);
            return;
        }

        totalSeconds--;

        const currentHours = Math.floor(totalSeconds / 3600);
        const currentMinutes = Math.floor((totalSeconds % 3600) / 60);

        // ✅ Гарантия, что при 1–59 сек будет отображаться "00:01"
        if (totalSeconds > 0 && totalSeconds < 60) {
            time.set("00:01");
        } else {
            const formatted = `${String(currentHours).padStart(2, '0')}:${String(currentMinutes).padStart(2, '0')}`;
            time.set(formatted);
        }
    }, 1000);
}

export function stopCountdown() {
    if (countdownInterval) {
        clearInterval(countdownInterval);
        countdownInterval = null;
    }
}
