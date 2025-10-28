const Natives = {
    SWITCH_OUT_PLAYER: '0xAAB3200ED59016BC',
    SWITCH_IN_PLAYER: '0xD8295AF639FD9CB8',
    IS_PLAYER_SWITCH_IN_PROGRESS: '0xD9D2CFFF49FAB35F'
};

gm.events.add('moveSkyCamera', moveFromToAir);

function moveFromToAir(player, moveTo, switchType) {   
   switch (moveTo) {
       case 'up':
            mp.game.invoke(Natives.SWITCH_OUT_PLAYER, player.handle, 0, parseInt(switchType));
           break;
       case 'down':
            mp.game.invoke(Natives.SWITCH_IN_PLAYER, player.handle);
           break;
   
       default:
           break;
   }
}

export const checkCamInAir = () => new Promise(async (resolve, reject) => {
    while(mp.game.invoke(Natives.IS_PLAYER_SWITCH_IN_PROGRESS)) {
        await global.wait(100);
    }

    return resolve(true);
});