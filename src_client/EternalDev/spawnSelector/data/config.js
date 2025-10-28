const CREATOR_CONFIG = {
    defaultCustomization: {
        mother: 21,
        father: 0,
        similarity: 0.5,
        skinSimilarity: 0.5,

        hair: 0,
        hairColor: [0, 0],

        eyeColor: 0,

        overlay: Array.from({ length: 13 }, (_, i) => ({ ...{ index: 255, opacity: 1, color1: 0, color2: 0 }})),
        faceFeatureParams: new Array(20).fill(0),

        clothes: {
            tops: {
                component: 11,
                drawable: -1,
                texture: 1
            },

            legs: {
                component: 4,
                drawable: -1,
                texture: 1
            },

            shoes: {
                component: 6,
                drawable: -1,
                texture: 2
            }
        }
    },

    clothesConfig: [ {
        tops: [ {
            component: 11,
            drawable: 1,
            texture: 1
        }, {
            component: 11,
            drawable: 5,
            texture: 0
        }, {
            component: 11,
            drawable: 22,
            texture: 1
        } ],
        legs: [ {
            component: 4,
            drawable: 43,
            texture: 1
        }, {
            component: 4,
            drawable: 5,
            texture: 2
        }, {
            component: 4,
            drawable: 5,
            texture: 0
        } ],
        shoes: [ {
            component: 6,
            drawable: 9,
            texture: 2
        }, {
            component: 6,
            drawable: 35,
            texture: 0
        }, {
            component: 6,
            drawable: 6,
            texture: 0
        } ]
    }, {
        tops: [ {
            component: 11,
            drawable: 32,
            texture: 2
        }, {
            component: 11,
            drawable: 2,
            texture: 7
        }, {
            component: 11,
            drawable: 49,
            texture: 1
        } ],
        legs: [ {
            component: 4,
            drawable: 4,
            texture: 13
        }, {
            component: 4,
            drawable: 12,
            texture: 8
        }, {
            component: 4,
            drawable: 14,
            texture: 1
        } ],
        shoes: [ {
            component: 6,
            drawable: 3,
            texture: 0
        }, {
            component: 6,
            drawable: 13,
            texture: 13
        }, {
            component: 6,
            drawable: 5,
            texture: 0
        } ]
    }],

    clothesAnimations: [
        'drop_clothes_a',
        'drop_clothes_b',
        'drop_clothes_c'
    ],
};

export default CREATOR_CONFIG;