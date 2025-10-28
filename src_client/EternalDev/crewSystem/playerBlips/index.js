import { CREW_VARIABLE_NAME, PLAYER_BLIPS_COLORS } from "../config";

const localPlayer = mp.players.local;

export default new class CrewPlayerBlips {
    pool = new Map();

    constructor() {
        mp.events.add({
            "entityStreamIn": this.onStreamIn.bind(this),
            "entityStreamOut": this.onStreamOut.bind(this),
            "playerQuit": (player) => {
                if (player.handle != localPlayer.handle)
                    return;

                this.destroyAllBlips();
            }
        });

        mp.events.addDataHandler(CREW_VARIABLE_NAME, this.onDataHandler.bind(this));
    }

    get localCrewData() {
        return this.getCrewData(localPlayer);
    }

    getCrewData(entity) {
        const data = entity.getVariable(CREW_VARIABLE_NAME);
        return data ? JSON.parse(data) : null;
    }

    onStreamIn(entity) {
        if (!entity || !entity.handle || entity.type != "player") 
            return;

        const localCrewData = this.localCrewData;
        if (localCrewData == null) return this.destroyIfExistsCrewPlayerBlip(entity);
        
        const entityCrewData = this.getCrewData(entity);
        if (entityCrewData == null || entityCrewData.Id != localCrewData.Id) return this.destroyIfExistsCrewPlayerBlip(entity);

        this.createOrUpdatePlayerBlip(entity);
    }

    onStreamOut(entity) {
        if (!entity || !entity.handle || entity.type != "player") 
            return;

        this.destroyIfExistsCrewPlayerBlip(entity);
    }

    onDataHandler(entity, data) {
        if (!entity || entity.type != "player" || !entity.handle) 
            return;

        const crewData = data ? JSON.parse(data) : null;
        if (entity == localPlayer) {
            if (crewData) {
                mp.players.forEachInStreamRange(targetPlayer => {
                    if (!targetPlayer || !targetPlayer.handle || targetPlayer.handle == localPlayer.handle) return;

                    const targetCrewData = this.getCrewData(targetPlayer);
                    if (targetCrewData == null || targetCrewData.Id != crewData.Id) return;

                    this.createOrUpdatePlayerBlip(targetPlayer);
                })
            }
            else {
                this.destroyAllBlips();
            }
        }
        else {
            crewData && this.localCrewData && this.localCrewData.Id == crewData.Id ? this.createOrUpdatePlayerBlip(entity) : this.destroyIfExistsCrewPlayerBlip(entity);
        }
    }

    destroyIfExistsCrewPlayerBlip(entity) {
        if (!entity || !this.pool.has(entity.remoteId)) return;

        const blip = this.pool.get(entity.remoteId);
        if (blip != null)
            mp.game.ui.removeBlip(blip);

        entity.crewData = null;
        this.pool.delete(entity.remoteId);
    }

    createOrUpdatePlayerBlip(entity) {
        if (!entity) return;

        const crewData = this.getCrewData(entity);
        if (crewData == null) return this.destroyIfExistsCrewPlayerBlip(entity);

        if (this.pool.has(entity.remoteId)) {
            if (entity.crewData == null || entity.crewData.Access == crewData.Access) 
                return;
            
            const blip = this.pool.get(entity.remoteId);
            mp.game.ui.setBlipColour(blip, PLAYER_BLIPS_COLORS[crewData.Access]);
        }
        else {
            const blip = mp.game.ui.addBlipForEntity(entity.handle);
            mp.game.ui.setBlipCategory(blip, 7);
            mp.game.ui.setBlipAsFriendly(blip, true);
            mp.game.ui.showHeadingIndicatorOnBlip(blip, true);
            mp.game.ui.setBlipColour(blip, PLAYER_BLIPS_COLORS[crewData.Access]);
    
            this.pool.set(entity.remoteId, blip);
        }

        entity.crewData = crewData;
    }

    destroyAllBlips() {
        this.pool.forEach((blip, remoteId) => {
            mp.game.ui.removeBlip(blip);
        });

        this.pool.clear();
    }
}