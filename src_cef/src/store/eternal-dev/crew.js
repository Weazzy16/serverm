import { writable } from 'svelte/store';

export const crewData = writable(
    null
    // {
    //     Id: 1,
    //     InviteCode: "X8X23",
    //     Members: {
    //         1: {
    //             UUID: 1,
    //             Access: 2,
    //             Name: "Ilya_Merumond",
    //             IsOnline: true 
    //         },
    //         4: {
    //             UUID: 4,
    //             Access: 0,
    //             Name: "Target_Player",
    //             IsOnline: false
    //         }
    //     }
    // }
);

window.crewStore = {
    setCrewData(value) {
        crewData.set(value)
    }
};