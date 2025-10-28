const call = mp.events.call;

export const HOOK = {
    SET_PAUSED: "e-dev.sounds.set_paused",
    SET_MUTED: "e-dev.sounds.set_muted",
    SET_PAUSED: "e-dev.sounds.set_paused",
    DESTROY: "e-dev.sounds.destroy",
    CREATED: "e-dev.sounds.created",
    UPDATE: "e-dev.sounds.uodate"
}

export const invokeHook = (hookNames, ...args) => {
    if (typeof(hookNames) != 'string')
        hookNames.forEach(hookName => {
            call(hookName, ...args);
        });
    else
        call(hookNames, ...args);
}

export const registerHook = (hookName, callback) => {
    gm.events.add(hookName, callback);
}