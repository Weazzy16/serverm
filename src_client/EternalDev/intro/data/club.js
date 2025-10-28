const scenario = {
    isPlaying: false,

    playerPosition: new mp.Vector3(417.57, -245.3952, 74.24924),
    time: {
        hour: 23,
        minute: 27,
        second: 0
    },
    weather: "CLEAR",

    ambientMusic: "intro.club",
    iplName: "mj_startscreen_autumn",

    scenes: {
        auth: {
            type: "interpolate",
            camPos: new mp.Vector3(409.9191, -257.8872, 72.40603),
            camLookAt: new mp.Vector3(409.4135, -259.2984, 72.40603),
            fov: 50
        },
        characters: {
            type: "interpolate",
            camPos: new mp.Vector3(417.0832, -254.6286, 73.61279),
            camLookAt: new mp.Vector3(409.0582, -258.4855, 70.23952),
            fov: 40,
            dof: {
                nearDof: 0,
                farDof: 150,
                strength: 1,
                shallowMode: true
            }
        },
        spawn: {
            type: "interpolate",
            camPos: new mp.Vector3(1347, -2648, 900),
            camLookAt: new mp.Vector3(-40.7, -760.14, 200),
            fov: 40,
        },
        customization: {
            type: "interpolate",
            camPos: new mp.Vector3(-763.93, 315.25, 217.66),
            camLookAt: new mp.Vector3(-765.45, 309.72, 217.29),
            fov: 40
        }
    },

    characters: {
        1: {
            position: new mp.Vector3(411.90036, -255.311295, 70.6865692),
            heading: -129.08078,
            camPos: new mp.Vector3(412.5383, -257.1826, 71.03494),
            camLookAt: new mp.Vector3(412.1273, -254.9181, 71.01143),
            fov: 40,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: true
            },
            freezePosition: true,
            animation: {
                animDict: "amb@prop_human_seat_chair@male@recline_b@base_b",
                animName: "base_b"
            }
        },
        2: {
            position: new mp.Vector3(404.985138, -254.812119, 71.2291565),
            heading: -111.395485,
            camPos: new mp.Vector3(406.5904, -255.4322, 71.2),
            camLookAt: new mp.Vector3(405.176, -255, 71.47),
            fov: 40,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: true
            },
            freezePosition: true,
            animation: {
                animDict: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animName: "li-mi_amb_club_06_base_male^3"
            }
        },
        3: {
            position: new mp.Vector3(412.447998, -260.310333, 71.2345886),
            heading: -37.4155846,
            camPos: new mp.Vector3(413.6208, -260.1866, 71.52526),
            camLookAt: new mp.Vector3(412.59, -260.6, 71.61),
            fov: 40,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: true
            },
            freezePosition: true,
            animation: {
                animDict: "amb@world_human_leaning@male@wall@back@legs_crossed@idle_a",
                animName: "idle_a"
            }
        }
    },

    defaultSceneDuration: 5000,
    cameras: [
        {
            from: {
                pos: new mp.Vector3(441.6356, -267.0901, 71.75332),
                pointAt: new mp.Vector3(431.3993, -258.3128, 70.30997),
                fov: 40,
                dof: {
                    nearDof: 2,
                    farDof: 5,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(435.7891, -266.0482, 91.25694),
                pointAt: new mp.Vector3(437.6809, -260.9068, 69.79926),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.3
                },
                fov: 43
            },
            to: {
                pos: new mp.Vector3(429.3665, -263.575, 91.25694),
                pointAt: new mp.Vector3(431.1938, -258.4684, 69.79926),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.3
                },
                fov: 43
            },
            duration: 6000
        },
        {
            from: {
                pos: new mp.Vector3(439.5769, -255.0066, 74.89699),
                pointAt: new mp.Vector3(440.4311, -256.7866, 74.89783),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.3
                },
                fov: 45,
                dof: {
                    nearDof: 0,
                    farDof: 3,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(422.3689, -263.0855, 71.50049),
                pointAt: new mp.Vector3(426.7594, -263.8123, 71.53094),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 33,
                dof: {
                    nearDof: 0,
                    farDof: 8,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(427.9396, -274.4458, 72.74697),
                pointAt: new mp.Vector3(434.9877, -255.6629, 73.25294),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.3
                },
                fov: 35,
                dof: {
                    nearDof: 5,
                    farDof: 30,
                    strength: 1,
                    shallowMode: true
                }
            },
            to: {
                pos: new mp.Vector3(429.9746, -269.0444, 72.74697),
                pointAt: new mp.Vector3(434.9877, -255.6629, 73.25294),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 35,
                dof: {
                    nearDof: 5,
                    farDof: 30,
                    strength: 1,
                    shallowMode: true
                }
            },
            duration: 6000
        },
        {
            from: {
                pos: new mp.Vector3(428.5129, -255.9117, 74.9686),
                pointAt: new mp.Vector3(429.5041, -253.3664, 74.94),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 40,
                dof: {
                    nearDof: 0,
                    farDof: 3,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(447.35, -262.55, 70.53816),
                pointAt: new mp.Vector3(444.57, -261.51, 70.56),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 30,
                dof: {
                    nearDof: 1,
                    farDof: 3,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(438.8867, -267.5786, 71.70633),
                pointAt: new mp.Vector3(440.2265, -268.5744, 71.45898),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.4
                },
                fov: 40,
                dof: {
                    nearDof: 0,
                    farDof: 2,
                    strength: 1,
                    shallowMode: true
                }
            },
            to: {
                pos: new mp.Vector3(439.4761, -267.371, 71.70633),
                pointAt: new mp.Vector3(440.2265, -268.5744, 71.45898),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 38,
                dof: {
                    nearDof: 0,
                    farDof: 2,
                    strength: 1,
                    shallowMode: true
                }
            },
            duration: 6000
        },
        {
            from: {
                pos: new mp.Vector3(438.1888, -263.4052, 70.64851),
                pointAt: new mp.Vector3(442.9548, -266.3503, 70.66),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 41,
                dof: {
                    nearDof: 0,
                    farDof: 3,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(419.4036, -258.7137, 71.18),
                pointAt: new mp.Vector3(420.3923, -255.7682, 71.1),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 40,
                dof: {
                    nearDof: 0,
                    farDof: 4,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(427.0503, -256.578, 71.1397),
                pointAt: new mp.Vector3(425.1963, -255.0862, 71.40678),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.5
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 4,
                    strength: 1,
                    shallowMode: true
                }
            },
            to: {
                pos: new mp.Vector3(425.9315, -257.1624, 71.4997),
                pointAt: new mp.Vector3(425.1963, -255.0862, 71.40678),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 4,
                    strength: 1,
                    shallowMode: true
                }
            },
            duration: 6000
        },
        {
            from: {
                pos: new mp.Vector3(428.3182, -255.3753, 71.05),
                pointAt: new mp.Vector3(430.4212, -253.6216, 70.94956),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 2.5,
                    strength: 1,
                    shallowMode: true
                }
            }
        }
    ],
    peds: [
        {
            model: "ig_sol",
            position: new mp.Vector3(439.874725, -268.82251, 71.2440109),
            heading: 15.6528492,
            freezePosition: false,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@djs@solomun@",
                animName: "temp_slmn_set_solomun"
            }
        },
        {
            model: "a_f_y_beach_01",
            position: new mp.Vector3(432.150787, -258.405701, 70.188797),
            heading: 158.965271,
            freezePosition: true,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@world_human_seat_steps@female@hands_by_sides@idle_b",
                animName: "idle_d"
            }
        },
        {
            model: "a_f_y_business_04",
            position: new mp.Vector3(419.396484, -257.572296, 71.1543427),
            heading: 65.5730667,
            freezePosition: true,
            variations: [{
                    componentId: 1,
                    textureId: 0
                }],
            animation: {
                animDict: "savecouch@",
                animName: "t_sleep_loop_couch"
            }
        },
        {
            model: "u_f_y_jewelass_01",
            position: new mp.Vector3(439.608948, -257.175537, 70.687767),
            heading: 175.693146,
            freezePosition: true,
            variations: [{
                    componentId: 1,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@prop_human_seat_chair@female@arms_folded@idle_a",
                animName: "idle_c"
            }
        },
        {
            model: "a_m_y_gay_01",
            position: new mp.Vector3(423.761963, -252.603699, 71.2541656),
            heading: 172.20047,
            freezePosition: false,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@world_human_leaning@male@wall@back@foot_up@idle_a",
                animName: "idle_c"
            }
        },
        {
            model: "a_m_y_jetski_01",
            position: new mp.Vector3(433.298828, -258.914093, 70.2670593),
            heading: 159.84407,
            freezePosition: true,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            scenario: "world_human_seat_wall"
        },
        {
            model: "u_m_m_willyfist",
            position: new mp.Vector3(439.717438, -256.098236, 74.5611343),
            heading: -28.8628311,
            freezePosition: true,
            variations: [{
                    componentId: 4,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@world_human_leaning@male@wall@back@legs_crossed@idle_a",
                animName: "idle_a"
            }
        },
        {
            model: "u_m_y_dancerave_01",
            position: new mp.Vector3(426.336273, -255.440826, 71.2468033),
            heading: 86.016983,
            freezePosition: false,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@podium_dancers@",
                animName: "hi_dance_facedj_17_v2_male^5"
            }
        },
        {
            model: "u_f_y_dancerave_01",
            position: new mp.Vector3(425.25351, -254.83139, 71.2434998),
            heading: -178.066101,
            freezePosition: false,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@podium_dancers@",
                animName: "hi_dance_facedj_17_v2_female^2"
            }
        },
        {
            model: "u_f_y_spyactress",
            position: new mp.Vector3(431.423462, -253.043427, 74.5441437),
            heading: 147.89209,
            freezePosition: true,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animName: "li-mi_amb_club_06_base_female^2"
            }
        },
        {
            model: "u_f_y_dancerave_01",
            position: new mp.Vector3(424.667114, -256.031097, 71.2532349),
            heading: -118.734421,
            freezePosition: false,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@crowddance_facedj@low_intesnsity",
                animName: "li_dance_facedj_11_v1_female^3"
            }
        },
        {
            model: "u_m_m_jesus_01",
            position: new mp.Vector3(430.385315, -253.357346, 74.6730042),
            heading: 159.463959,
            freezePosition: true,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@world_human_seat_wall@male@hands_in_lap@base",
                animName: "base"
            }
        },
        {
            model: "u_m_m_partytarget",
            position: new mp.Vector3(430.678558, -254.065399, 71.690033),
            heading: 83.1689987,
            freezePosition: true,
            variations: [{
                    componentId: 1,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_STUPOR"
        },
        {
            model: "cs_jimmydisanto",
            position: new mp.Vector3(445.069763, -261.13382, 71.235611),
            heading: 164.597031,
            freezePosition: false,
            variations: [{
                    componentId: 5,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_STUPOR"
        },
        {
            model: "a_f_y_tourist_01",
            position: new mp.Vector3(442.584229, -258.425232, 71.2430878),
            heading: -140.096054,
            freezePosition: false,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_SMOKING_POT"
        },
        {
            model: "a_f_y_hipster_02",
            position: new mp.Vector3(439.297089, -264.426544, 71.1949387),
            heading: 79.7905655,
            freezePosition: true,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_PICNIC"
        },
        {
            model: "u_f_y_dancerave_01",
            position: new mp.Vector3(443.411011, -259.25296, 71.2518387),
            heading: 26.718441,
            freezePosition: false,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_HANG_OUT_STREET"
        },
        {
            model: "u_m_y_dancerave_01",
            position: new mp.Vector3(424.035767, -264.050537, 71.250206),
            heading: -55.8904915,
            freezePosition: false,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_STAND_MOBILE"
        },
        {
            model: "a_m_y_vinewood_01",
            position: new mp.Vector3(423.012695, -259.551514, 71.2333832),
            heading: -34.9930573,
            freezePosition: false,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_STAND_MOBILE"
        },
        {
            model: "a_m_m_mlcrisis_01",
            position: new mp.Vector3(420.966309, -256.126526, 71.2568207),
            heading: -121.275101,
            freezePosition: true,
            variations: [{
                    componentId: 5,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@scripted@carmeet@tun_meet_ig2_race@",
                animName: "base"
            }
        },
        {
            model: "a_m_y_stbla_02",
            position: new mp.Vector3(417.077698, -260.188446, 70.6770935),
            heading: -102.026817,
            freezePosition: true,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@prop_human_seat_chair@male@generic_skinny@base",
                animName: "base"
            }
        },
        {
            model: "u_m_y_dancerave_01",
            position: new mp.Vector3(441.412384, -261.795166, 71.2452087),
            heading: -160.691208,
            freezePosition: false,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@black_madonna_entourage@",
                animName: "hi_dance_facedj_09_v2_male^5"
            }
        },
        {
            model: "a_m_y_vinewood_04",
            position: new mp.Vector3(441.567719, -263.092957, 71.2465134),
            heading: 12.887846,
            freezePosition: false,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@crowddance_facedj@low_intesnsity",
                animName: "li_dance_facedj_13_v2_male^4"
            }
        },
        {
            model: "a_m_y_business_02",
            position: new mp.Vector3(436.75412, -258.801727, 71.254631),
            heading: 81.7725601,
            freezePosition: false,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub_island@dancers@crowddance_facedj_transitions@",
                animName: "trans_dance_facedj_mi_to_li_09_v1_male^5"
            }
        },
        {
            model: "u_f_y_dancerave_01",
            position: new mp.Vector3(442.691376, -262.228912, 71.2429428),
            heading: 127.189468,
            freezePosition: false,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@crowddance_facedj@",
                animName: "hi_dance_facedj_09_v1_female^3"
            }
        },
        {
            model: "a_f_y_hipster_03",
            position: new mp.Vector3(432.228302, -254.580231, 71.2525864),
            heading: -101.496841,
            freezePosition: false,
            variations: [{
                    componentId: 4,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@club_ambientpeds@low-med_intensity",
                animName: "li-mi_amb_club_09_v1_female^3"
            }
        },
        {
            model: "a_f_y_vinewood_04",
            position: new mp.Vector3(436.243805, -258.326721, 71.2500229),
            heading: 162.069061,
            freezePosition: false,
            variations: [{
                    componentId: 4,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@crowddance_groups@hi_intensity",
                animName: "hi_dance_crowd_09_v1_female^1"
            }
        }
    ],

    vehicles: [],

    deleteObjects: [
        {
            x: 413.3616,
            y: -255.1277,
            z: 70.23516,
            radius: 1,
            model: 2580107545
        },
        {
            x: 410.4957,
            y: -259.38,
            z: 70.23577,
            radius: 1,
            model: 977744387
        },
        {
            x: 406.1225,
            y: -255.0734,
            z: 70.64162,
            radius: 1,
            model: 702477265
        },
        {
            x: 406.1866,
            y: -256.1353,
            z: 70.24013,
            radius: 1,
            model: 2533307946
        },
        {
            x: 407.1402,
            y: -255.0118,
            z: 70.24013,
            radius: 1,
            model: 2533307946
        },
        {
            x: 406.0504,
            y: -253.9627,
            z: 70.24013,
            radius: 1,
            model: 2533307946
        },
        {
            x: 415.6478,
            y: -250.0494,
            z: 70.62975,
            radius: 1,
            model: 4087940966
        },
        {
            x: 418.9924,
            y: -251.3105,
            z: 70.64227,
            radius: 1,
            model: 4087940966
        },
        {
            x: 431.9688,
            y: -255.5809,
            z: 70.2565,
            radius: 1,
            model: 3702106121
        },
        {
            x: 434.5309,
            y: -253.83,
            z: 73.507,
            radius: 1,
            model: 2580107545
        },
        {
            x: 427.7083,
            y: -251.3617,
            z: 73.925,
            radius: 1,
            model: 4087940966
        },
        {
            x: 428.4743,
            y: -251.6326,
            z: 73.50294,
            radius: 1,
            model: 2533307946
        },
        {
            x: 427.0161,
            y: -251.1125,
            z: 73.50294,
            radius: 1,
            model: 2533307946
        }
    ],
};

export default scenario;