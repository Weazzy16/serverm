import { SoundEntity } from "./entity";

const browsers = mp.browsers;
export default new class SoundManager {
    browser = browsers.new("http://package/e-dev/audio/index.html")
    pool = {}

    constructor() {
        setInterval(() => this.worker(), 100);

        gm.events.add("e-dev.sound_manager.create3d", this.create3d.bind(this));
        gm.events.add("e-dev.sound_manager.stop3d", this.stop3d.bind(this));
        gm.events.add("e-dev.sound_manager.pause3d", this.setPause3d.bind(this));
        gm.events.add("e-dev.sound_manager.change_volume", this.changeVolume3d.bind(this));
    }

    /**
     * Получить SoundEntity по индификатору
     * @param {Индификатор} id 
     * @returns SoundEntity
     */
    getSound(id) {
        return this.pool[id];
    }

    /**
     * Получить SoundEntity по Handle сущности
     * @param {Entity handle} handle 
     * @returns SoundEntity
     */
    getSoundByHandle(handle) {
        return Object.values(this.pool).find(x => x.entity.handle == handle) || null;
    }
    
    /**
     * Создать звук в пространстве, привязанный к определнному entity
     * @param {RAGE Entity} entity 
     * @param {Данные о звуке} soundData 
     */
    create3d(entity, soundData) {
        const data = typeof soundData == "string" 
            ? JSON.parse(soundData) : soundData;
    
        const soundEntity = new SoundEntity(this.browser, data.id, data.url, data.type, entity, {
            looped: data.looped, 
            startOffset: data.startOffset, 
            volume: data.volume, 
            maxDistance: data.maxDistance,
            isPausing: data.isPausing
        });
    
        this.pool[data.id] = soundEntity;
    }

    stop3d(id) {
        const sound = this.getSound(id);
        if (sound == null) return;

        sound.destroy();
        delete this.pool[id];
    }

    setPause3d(id, toggle) {
        const sound = this.getSound(id);
        if (sound == null) return;

        sound.paused = toggle;
    }

    changeVolume3d(id, volume) {
        const sound = this.getSound(id);
        if (sound == null) return;

        sound.volume = volume;
    }

    worker() {
        if (this.browser == null)
            return;

        for (const [id, entity] of Object.entries(this.pool)) {
            entity.worker();
        } 
    }
}