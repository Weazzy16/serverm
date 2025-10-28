export const scenario = {
    playerPos: new mp.Vector3(3301.14722, 5176.35205, 19.484026),

    time: {
        hour: 21,
        minute: 0,
        second: 0
    },
    weather: "CLEAR",
    snow: !0,
    ambientMusic: "intro.xmas2",
    iplName: "mj_starthouse",

    defaultSceneDuration: 5000,

    scenes: {
        auth: {
            type: "interpolate",
            camPos: new mp.Vector3(3308.45288, 5183.17822, 20.0426216),
            camLookAt: new mp.Vector3(3306.30005, 5184.81982, 20.0133896),
            fov: 30,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: !0
            }
        },
        characters: {
            type: "interpolate",
            camPos: new mp.Vector3(3310.68311, 5174.59863, 24.8903923),
            camLookAt: new mp.Vector3(3305.6084, 5174.64062, 22.98102),
            fov: 50,
            dof: {
                nearDof: 0,
                farDof: 7,
                strength: 1,
                shallowMode: !0
            }
        },
        spawn: {
            type: "interpolate",
            camPos: new mp.Vector3(1347, -2648, 900),
            camLookAt: new mp.Vector3(-40.7, -760.14, 200),
            fov: 40
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
            position: new mp.Vector3(3306.26172, 5177.0835, 22.8613281),
            rotation: new mp.Vector3(0, 0, -24.6479092),
            camPos: new mp.Vector3(3307.4729, 5176.33252, 23.43894),
            camLookAt: new mp.Vector3(3306.52856, 5177.65576, 23.2798386),
            fov: 40,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: !0
            },
            freezePosition: !0,
            animation: {
                animDict: "amb@prop_human_seat_computer@male@idle_a",
                animName: "idle_c"
            }
        },
        2: {
            position: new mp.Vector3(3307.57056, 5173.46045, 22.9647179),
            rotation: new mp.Vector3(0, 0, -21.5649986),
            camPos: new mp.Vector3(3308.79199, 5174.51758, 23.0948353),
            camLookAt: new mp.Vector3(3303.23804, 5171.45654, 24.1914005),
            fov: 40,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: !0
            },
            freezePosition: !0,
            animation: {
                animDict: "amb@prop_human_seat_chair@male@recline_b@base_b",
                animName: "base_b"
            }
        },
        3: {
            position: new mp.Vector3(3304.49951, 5171.33789, 24.1063786),
            rotation: new mp.Vector3(0, 0, -118.307938),
            camPos: new mp.Vector3(3305.02515, 5169.80762, 23.375845),
            camLookAt: new mp.Vector3(3304.9917, 5171.14844, 23.5323601),
            fov: 40,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: !0
            },
            freezePosition: !0,
            animation: {
                animDict: "anim@amb@business@bgen@bgen_no_work@",
                animName: "sit_phone_phoneputdown_idle_nowork"
            }
        }
    },

    cameras: [ 
        {
            from: {
                pos: new mp.Vector3(3301.14722, 5176.35205, 19.484026),
                pointAt: new mp.Vector3(3300.97681, 5175.45215, 19.4872799),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .2
                },
                fov: 50,
                dof: {
                    nearDof: 1,
                    farDof: 3,
                    strength: 1,
                    shallowMode: !0
                }
            }
        }, {
            from: {
                pos: new mp.Vector3(3309.06421, 5182.146, 19.9708157),
                pointAt: new mp.Vector3(3308.29077, 5181.62012, 19.9839725),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .2
                },
                fov: 40,
                dof: {
                    nearDof: 0,
                    farDof: 5,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 4e3
        }, {
            from: {
                pos: new mp.Vector3(3306.4397, 5178.74707, 19.5877438),
                pointAt: new mp.Vector3(3305.17603, 5177.90088, 19.6398258),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .4
                },
                fov: 40,
                dof: {
                    nearDof: 1,
                    farDof: 4,
                    strength: 1,
                    shallowMode: !0
                }
            },
            to: {
                pos: new mp.Vector3(3305.46997, 5179.68311, 20.0977554),
                pointAt: new mp.Vector3(3304.7561, 5177.1709, 19.8562908),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .5
                },
                fov: 35,
                dof: {
                    nearDof: 1,
                    farDof: 6,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 8e3
        }, {
            from: {
                pos: new mp.Vector3(3304, 5180.76416, 19.1540184),
                pointAt: new mp.Vector3(3302.8916, 5180.82861, 19.1959534),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .2
                },
                fov: 30,
                dof: {
                    nearDof: 0,
                    farDof: 2,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 4e3
        }, {
            from: {
                pos: new mp.Vector3(3298.63403, 5174.28076, 23.6242943),
                pointAt: new mp.Vector3(3296.86108, 5177.31299, 23.4286804),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .2
                },
                fov: 40,
                dof: {
                    nearDof: 1,
                    farDof: 5,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 4e3
        }, {
            from: {
                pos: new mp.Vector3(3299.49048, 5180.04736, 18.8240108),
                pointAt: new mp.Vector3(3300.23193, 5178.88965, 18.8189583),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .1
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 2,
                    strength: 1,
                    shallowMode: !0
                }
            },
            to: {
                pos: new mp.Vector3(3299.49048, 5180.04736, 18.8240108),
                pointAt: new mp.Vector3(3298.98901, 5177.49365, 19.3065968),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .1
                },
                fov: 35,
                dof: {
                    nearDof: 1,
                    farDof: 4,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 7e3
        }, {
            from: {
                pos: new mp.Vector3(3309.93823, 5183.21484, 20.0040379),
                pointAt: new mp.Vector3(3310.96997, 5185.29688, 20.0030098),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .2
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 3,
                    strength: 1,
                    shallowMode: !0
                }
            }
        }, {
            from: {
                pos: new mp.Vector3(3301.86865, 5170.89453, 23.3784199),
                pointAt: new mp.Vector3(3301.95947, 5172.65771, 23.4355202),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 40,
                dof: {
                    nearDof: 0,
                    farDof: 4,
                    strength: 1,
                    shallowMode: !0
                }
            }
        }, {
            from: {
                pos: new mp.Vector3(3296.71997, 5176.90771, 19.8940353),
                pointAt: new mp.Vector3(3306.66992, 5176.1416, 19.8362045),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 38,
                dof: {
                    nearDof: 0,
                    farDof: 5,
                    strength: 1,
                    shallowMode: !0
                }
            },
            to: {
                pos: new mp.Vector3(3297.15039, 5176.64062, 21.2361584),
                pointAt: new mp.Vector3(3297.68848, 5176.25049, 21.2876053),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 38,
                dof: {
                    nearDof: 0,
                    farDof: 1,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 6e3
        }, {
            from: {
                pos: new mp.Vector3(3297.15039, 5176.64062, 21.2361584),
                pointAt: new mp.Vector3(3297.68848, 5176.25049, 21.2876053),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 38,
                dof: {
                    nearDof: 0,
                    farDof: 1,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 2e3
        }, {
            from: {
                pos: new mp.Vector3(3299.86084, 5172.47852, 19.1640186),
                pointAt: new mp.Vector3(3302.76685, 5172.31689, 19.2804966),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 38,
                dof: {
                    nearDof: 0,
                    farDof: 3,
                    strength: 1,
                    shallowMode: !0
                }
            }
        }, {
            from: {
                pos: new mp.Vector3(3304.54443, 5170.37695, 19.5930557),
                pointAt: new mp.Vector3(3307.68433, 5170.93311, 19.7903786),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .2
                },
                fov: 40,
                dof: {
                    nearDof: 0,
                    farDof: 8,
                    strength: 1,
                    shallowMode: !0
                }
            }
        }, {
            from: {
                pos: new mp.Vector3(3306.94531, 5177.08398, 21.1910801),
                pointAt: new mp.Vector3(3306.7168, 5178.30762, 21.0238266),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 20,
                dof: {
                    nearDof: 0,
                    farDof: 1,
                    strength: 1,
                    shallowMode: !0
                }
            },
            to: {
                pos: new mp.Vector3(3306.94531, 5177.08398, 21.1910801),
                pointAt: new mp.Vector3(3303.40479, 5182.72852, 20.713604),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 20,
                dof: {
                    nearDof: 1,
                    farDof: 10,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 4e3
        }, {
            from: {
                pos: new mp.Vector3(3306.94531, 5177.08398, 21.1910801),
                pointAt: new mp.Vector3(3303.40479, 5182.72852, 20.713604),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: .3
                },
                fov: 20,
                dof: {
                    nearDof: 1,
                    farDof: 10,
                    strength: 1,
                    shallowMode: !0
                }
            },
            duration: 2e3
        } 
    ],

    peds: [ {
        model: "a_c_cat_01",
        position: new mp.Vector3(3300.7937, 5175.38281, 19.3612022),
        rotation: new mp.Vector3(-605892501e-14, -0, -155.628113),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "creatures@cat@amb@world_cat_sleeping_ground@base",
            animName: "base"
        }
    }, {
        model: "g_f_importexport_01",
        position: new mp.Vector3(3300.41357, 5175.35107, 20.0763283),
        rotation: new mp.Vector3(0, -0, -134.862579),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "anim@amb@business@bgen@bgen_no_work@",
            animName: "sit_phone_idle_01_nowork"
        }
    }, {
        model: "a_m_y_bevhills_01",
        position: new mp.Vector3(3303.01685, 5172.32129, 19.6163921),
        rotation: new mp.Vector3(0, -0, -160.31604),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        scenario: "WORLD_HUMAN_AA_COFFEE"
    }, {
        model: "a_f_y_tourist_01",
        position: new mp.Vector3(3302.62964, 5171.29248, 19.1639843),
        rotation: new mp.Vector3(0, 0, -43.0577049),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "amb@prop_human_seat_chair@female@arms_folded@idle_a",
            animName: "idle_b"
        }
    }, {
        model: "a_f_y_business_04",
        position: new mp.Vector3(3304.68066, 5178.32324, 19.6449108),
        rotation: new mp.Vector3(-.0433241427, -.000386908068, -138.585892),
        freezePosition: !0,
        variations: [ {
            componentId: 0,
            textureId: 0
        } ],
        animation: {
            animDict: "anim@amb@nightclub@dancers@crowddance_facedj@",
            animName: "mi_dance_facedj_17_v1_female^4"
        }
    }, {
        model: "u_f_y_spyactress",
        position: new mp.Vector3(3305.90527, 5170.14502, 19.1602097),
        rotation: new mp.Vector3(0, 0, 46.145359),
        freezePosition: !0,
        variations: [ {
            componentId: 0,
            textureId: 0
        } ],
        animation: {
            animDict: "amb@world_human_seat_steps@female@hands_by_sides@idle_a",
            animName: "idle_a"
        }
    }, {
        model: "a_f_y_bevhills_01",
        position: new mp.Vector3(3296.89917, 5176.70654, 23.4442654),
        rotation: new mp.Vector3(0, -0, 62.4663849),
        freezePosition: !0,
        variations: [ {
            componentId: 1,
            textureId: 0
        } ],
        scenario: "WORLD_HUMAN_STAND_MOBILE"
    }, {
        model: "u_f_y_lauren",
        position: new mp.Vector3(3304.78003, 5176.72412, 19.6582737),
        rotation: new mp.Vector3(0, -0, 17.3477669),
        freezePosition: !0,
        variations: [ {
            componentId: 1,
            textureId: 0
        } ],
        animation: {
            animDict: "anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity",
            animName: "hi_dance_facedj_17_v2_female^3"
        }
    }, {
        model: "u_f_y_mistress",
        position: new mp.Vector3(3311.14526, 5185.47559, 19.6147366),
        rotation: new mp.Vector3(0, -0, 157.963928),
        freezePosition: !0,
        variations: [ {
            componentId: 2,
            textureId: 0
        } ],
        animation: {
            animDict: "anim@amb@nightclub@dancers@club_ambientpeds@",
            animName: "li-mi_amb_club_06_base_female^2"
        }
    }, {
        model: "a_f_y_vinewood_04",
        position: new mp.Vector3(3305.75439, 5174.19678, 19.1663857),
        rotation: new mp.Vector3(-1.06623716e-7, -159027747e-23, 168.74086),
        freezePosition: !0,
        variations: [ {
            componentId: 3,
            textureId: 0
        } ],
        animation: {
            animDict: "amb@prop_human_seat_chair@female@legs_crossed@idle_b",
            animName: "idle_d"
        }
    }, {
        model: "a_f_y_soucent_03",
        position: new mp.Vector3(3298.46582, 5175.23438, 23.1018276),
        rotation: new mp.Vector3(-.541902244, 8.00448561e-8, 51.308342),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "amb@world_human_seat_steps@female@hands_by_sides@idle_b",
            animName: "idle_d"
        }
    }, {
        model: "u_f_y_jewelass_01",
        position: new mp.Vector3(3311.20239, 5184.35986, 19.6157475),
        rotation: new mp.Vector3(0, 0, 32.7526894),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "missheistdockssetup1ig_10@idle_c",
            animName: "talk_pipe_c_worker2"
        }
    }, {
        model: "u_f_y_hotposh_01",
        position: new mp.Vector3(3308.02271, 5181.35352, 19.4632721),
        rotation: new mp.Vector3(0, 0, -33.701107),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "amb@prop_human_seat_bar@female@elbows_on_bar@idle_a",
            animName: "idle_b"
        }
    }, {
        model: "a_f_y_yoga_01",
        position: new mp.Vector3(3302.13208, 5172.83838, 23.3660946),
        rotation: new mp.Vector3(0, 0, 47.9974709),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "anim@mp_bedmid@left_var_02",
            animName: "f_sleep_l_loop_bighouse"
        }
    }, {
        model: "a_m_m_bevhills_02",
        position: new mp.Vector3(3303.18799, 5182.85303, 20.4526691),
        rotation: new mp.Vector3(0, -0, -127.81752),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "amb@world_human_seat_steps@male@elbows_on_knees@idle_a",
            animName: "idle_a"
        }
    }, {
        model: "a_m_y_bevhills_02",
        position: new mp.Vector3(3308.97998, 5179.32031, 19.6153851),
        rotation: new mp.Vector3(0, 0, 42.7566147),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "amb@world_human_leaning@female@wall@back@holding_elbow@idle_a",
            animName: "idle_a"
        }
    }, {
        model: "a_m_y_business_02",
        position: new mp.Vector3(3305.29272, 5177.78516, 19.6450558),
        rotation: new mp.Vector3(0, -0, 141.035919),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        scenario: "WORLD_HUMAN_PARTYING"
    }, {
        model: "a_m_y_jetski_01",
        position: new mp.Vector3(3302.54126, 5173.48242, 23.4469013),
        rotation: new mp.Vector3(-36283343e-13, 407110977e-21, 53.669796),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "anim@mp_bedmid@left_var_04",
            animName: "f_sleep_l_loop_bighouse"
        }
    }, {
        model: "a_m_m_skater_01",
        position: new mp.Vector3(3300.58594, 5178.55615, 19.6211929),
        rotation: new mp.Vector3(937666913e-23, -189575822e-30, 72.7882462),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        scenario: "WORLD_HUMAN_SUNBATHE"
    }, {
        model: "a_m_y_latino_01",
        position: new mp.Vector3(3303.98633, 5177.26465, 19.6680241),
        rotation: new mp.Vector3(0, -0, -72.0678253),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "anim@amb@nightclub@dancers@crowddance_facedj@",
            animName: "hi_dance_facedj_09_v1_male^2"
        }
    }, {
        model: "a_c_westy",
        position: new mp.Vector3(3302.77856, 5180.86719, 19.1998558),
        rotation: new mp.Vector3(0, -0, -140.973251),
        freezePosition: !0,
        variations: [ {
            componentId: 4,
            textureId: 0
        } ],
        animation: {
            animDict: "creatures@pug@amb@world_dog_sitting@idle_a",
            animName: "idle_b"
        }
    } ],

    vehicles: [],
    deleteObjects: [],
    disableStaticEmitters: [ "collision_781bnhb", "collision_8cue4t5" ],
}