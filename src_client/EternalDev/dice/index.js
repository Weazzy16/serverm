export default new class Dice {
    constructor() {
        mp.attachments.register(
            "dice_cube_1", 
            "mj_dice_zero", 
            6286, 
            new mp.Vector3(0, 0, 0),
            new mp.Vector3(-90, 0, 0), 
            true
        );

        mp.attachments.register(
            "dice_cube_2", 
            "mj_dice_zero", 
            6286, 
            new mp.Vector3(0, 0, 0),
            new mp.Vector3(0, 0, 0), 
            true
        );

        mp.attachments.register(
            "dice_cube_3", 
            "mj_dice_zero", 
            6286, 
            new mp.Vector3(0, 0, 0),
            new mp.Vector3(0, 0, -90), 
            true
        );

        mp.attachments.register(
            "dice_cube_4", 
            "mj_dice_zero", 
            6286, 
            new mp.Vector3(0, 0, 0),
            new mp.Vector3(0, 0, 90), 
            true
        );

        mp.attachments.register(
            "dice_cube_5", 
            "mj_dice_zero", 
            6286, 
            new mp.Vector3(0, 0, 0),
            new mp.Vector3(0, 0, -180), 
            true
        );

        mp.attachments.register(
            "dice_cube_6", 
            "mj_dice_zero", 
            6286, 
            new mp.Vector3(0, 0, 0),
            new mp.Vector3(90, 0, 0), 
            true
        );
    }
}