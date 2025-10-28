// PETSHOP
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let petshopcam = null;
let petModels = null;
let petHashes = null;

let pet = {
    model: null,
    entity: null,
	dimension: 0,
}

gm.events.add('openPetshop', async (models, hashes, prices, dim) => {
    try
    {
        if (global.menuCheck()) return;
        
        petModels = JSON.parse(models);
        petHashes = JSON.parse(hashes);
        
        global.menuOpen();
        mp.gui.emmit(`window.router.setView("BusinessPetShop")`);
        await global.wait(50); 

        setPet('models', models);
        setPet('hashes', hashes);
        setPet('prices', prices);
        
        pet.entity = mp.peds.new(petHashes[0], new mp.Vector3(-758.2859, 320.9569, 175.2784), 218.8, dim);
        pet.dimension = dim;
        global.localplayer.setRotation(0, 0, 0, 2, true);
        pet.model = petModels[0];
        
        global.createCamera ("petshop");
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/petshop", "openPetshop", e.toString());
    }
});

gm.events.add('closePetshop', () => {
    try
    {
        if(new Date().getTime() - global.lastCheck < 50) return; 
        global.lastCheck = new Date().getTime();

        callRemote('petshopCancel');

        destroyPetShop ();
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/petshop", "closePetshop", e.toString());
    }
});

function setPet(type, jsonstr) {
    mp.gui.emmit(`window.petshop.setVariable("${type}", '${jsonstr}')`);
}

gm.events.add('petshop', (act, value) => {
    try
    {
        switch (act) {
            case "model":
                pet.model = petModels[value];
                if(pet.entity != null) {
                    pet.entity.destroy();
                    pet.entity = mp.peds.new(petHashes[value], new mp.Vector3(-758.2859, 320.9569, 175.2784), 218.8, pet.dimension);
                }
                break;
        }
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/petshop", "petshop", e.toString());
    }
});

gm.events.add('buyPet', () => {
    try
    {
        if(new Date().getTime() - global.lastCheck < 50) return; 
        global.lastCheck = new Date().getTime();

        callRemote('petshopBuy', pet.model);

        destroyPetShop ();
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/petshop", "buyPet", e.toString());
    }
});


const destroyPetShop = () => {
    global.menuClose();
    mp.gui.emmit('window.router.setHud()');
    global.cameraManager.stopCamera ();
    if (pet.entity == null) return;
    pet.entity.destroy();
    pet.entity = null;
};