const scenario = {
    playerPosition: new mp.Vector3(3278.569, 5203.673, 18.43876),
    time: {
        hour: 6,
        minute: 30,
        second: 0
    },
    weather: "XMAS",
    ambientMusic: "intro.xmas",
    iplName: "MJ_StartScreen_Winter",

    defaultSceneDuration: 5000,
	
	scenes: {
		auth: {
			type: "interpolate",
			camPos: new mp.Vector3(3304.278, 5196.505, 21.5713),
			camLookAt: new mp.Vector3(3304.811, 5197.596, 21.59862),
			fov: 40,
			dof: {
				nearDof: 0,
				farDof: 2,
				strength: 1,
				shallowMode: true
			}
		},
		characters: {
			type: "interpolate",
			camPos: new mp.Vector3(3307.514, 5194.813, 20.48525),
			camLookAt: new mp.Vector3(3299.062, 5194.285, 17.41537),
			fov: 45,
			dof: {
				nearDof: 0,
				farDof: 10,
				strength: 1,
				shallowMode: true
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
			position: new mp.Vector3(3302.15137, 5197.1001, 18.2593899),
			heading: -125.834648,
			camPos: new mp.Vector3(3302.847, 5195.416, 18.59429),
			camLookAt: new mp.Vector3(3302.713, 5198.073, 18.39352),
			fov: 50,
			dof: {
				nearDof: 0,
				farDof: 2,
				strength: 1,
				shallowMode: true
			},
			freezePosition: true,
			animation: {
				animDict: "amb@world_human_seat_steps@male@hands_in_lap@idle_a",
				animName: "idle_b"
			}
		},
		2: {
			position: new mp.Vector3(3301.13623, 5190.49365, 17.8953476),
			heading: 11.7326546,
			camPos: new mp.Vector3(3302.036, 5192.251, 17.96983),
			camLookAt: new mp.Vector3(3300.822, 5191.027, 17.97577),
			fov: 50,
			dof: {
				nearDof: 0,
				farDof: 2,
				strength: 1,
				shallowMode: true
			},
			freezePosition: true,
			animation: {
				animDict: "anim@amb@clubhouse@seating@female@var_a@base@",
				animName: "base"
			}
		},
		3: {
			position: new mp.Vector3(3297.04004, 5194.85596, 18.41535),
			heading: 95.5911255,
			camPos: new mp.Vector3(3295.443, 5193.246, 18.34907),
			camLookAt: new mp.Vector3(3296.173, 5194.407, 18.44291),
			fov: 50,
			dof: {
				nearDof: 0,
				farDof: 2,
				strength: 1,
				shallowMode: true
			},
			freezePosition: true,
			animation: {
				animDict: "misscarsteal2fixer",
				animName: "confused_a"
			}
		}
	},
	
    cameras: [
        {
            from: {
                pos: new mp.Vector3(3343.965, 5168.781, 21.11119),
                pointAt: new mp.Vector3(3333.17, 5165.957, 21.12037),
                fov: 45,
                dof: {
                    nearDof: 1,
                    farDof: 30,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(3268.336, 5211.413, 18.65537),
                pointAt: new mp.Vector3(3269.43, 5211.533, 18.62723),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 50,
                dof: {
                    nearDof: 0,
                    farDof: 2,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(3268.926, 5192.617, 45.67708),
                pointAt: new mp.Vector3(3306.289, 5159.876, 17.41537),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.3
                },
                fov: 43,
                dof: {
                    nearDof: 1,
                    farDof: 40,
                    strength: 1,
                    shallowMode: true
                }
            },
            to: {
                pos: new mp.Vector3(3267.863, 5191.196, 45.67708),
                pointAt: new mp.Vector3(3309.299, 5170.49, 30.03579),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.3
                },
                fov: 43,
                dof: {
                    nearDof: 1,
                    farDof: 40,
                    strength: 1,
                    shallowMode: true
                }
            },
            duration: 6000
        },
        {
            from: {
                pos: new mp.Vector3(3318.321, 5165.814, 17.56521),
                pointAt: new mp.Vector3(3320.426, 5166.352, 17.79021),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 45,
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
                pos: new mp.Vector3(3350.077, 5150.195, 20.04964),
                pointAt: new mp.Vector3(3352.283, 5151.631, 20.17905),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 42,
                dof: {
                    nearDof: 1,
                    farDof: 10,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(3334.46, 5165.657, 21.67722),
                pointAt: new mp.Vector3(3334.029, 5162.108, 17.32055),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.3
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 2,
                    strength: 1,
                    shallowMode: true
                }
            },
            to: {
                pos: new mp.Vector3(3332.49, 5164.639, 21.67722),
                pointAt: new mp.Vector3(3334.029, 5162.108, 17.32055),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 35,
                dof: {
                    nearDof: 1,
                    farDof: 4,
                    strength: 1,
                    shallowMode: true
                }
            },
            duration: 6000
        },
        {
            from: {
                pos: new mp.Vector3(3297.795, 5141.248, 18.55378),
                pointAt: new mp.Vector3(3308.806, 5145.54, 17.30857),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 40,
                dof: {
                    nearDof: 1,
                    farDof: 25,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(3322.894, 5135.708, 26.22259),
                pointAt: new mp.Vector3(3315.276, 5174.919, 17.86705),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 30,
                dof: {
                    nearDof: 0,
                    farDof: 30,
                    strength: 1,
                    shallowMode: true
                }
            }
        },
        {
            from: {
                pos: new mp.Vector3(3320.902, 5170.535, 25.05699),
                pointAt: new mp.Vector3(3323.902, 5173.854, 24.89101),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.4
                },
                fov: 40,
                dof: {
                    nearDof: 0,
                    farDof: 7,
                    strength: 1,
                    shallowMode: true
                }
            },
            to: {
                pos: new mp.Vector3(3321.038, 5173.027, 18.01521),
                pointAt: new mp.Vector3(3323.44, 5175.569, 18.47395),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 38,
                dof: {
                    nearDof: 0,
                    farDof: 7,
                    strength: 1,
                    shallowMode: true
                }
            },
            duration: 6000
        },
        {
            from: {
                pos: new mp.Vector3(3295.012, 5192.766, 17.71537),
                pointAt: new mp.Vector3(3293.888, 5192.356, 17.76823),
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
                pos: new mp.Vector3(3200.353, 5234.18, 24.553),
                pointAt: new mp.Vector3(3203.352, 5230.677, 24.49544),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 50,
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
                pos: new mp.Vector3(3313.457, 5177.349, 20.06018),
                pointAt: new mp.Vector3(3329.281, 5159.19, 17.49922),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.5
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 10,
                    strength: 1,
                    shallowMode: true
                }
            },
            to: {
                pos: new mp.Vector3(3314.486, 5178.216, 20.06018),
                pointAt: new mp.Vector3(3320.559, 5165.96, 17.45631),
                shake: {
                    type: "HAND_SHAKE",
                    amplitude: 0.2
                },
                fov: 35,
                dof: {
                    nearDof: 0,
                    farDof: 10,
                    strength: 1,
                    shallowMode: true
                }
            },
            duration: 6000
        }
    ],

    peds: [
        {
            model: "a_m_m_eastsa_02",
            position: new mp.Vector3(3370.22852, 5184.81201, 1.45543039),
            heading: -14.2515755,
            freezePosition: false,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_STAND_FISHING"
        },
        {
            model: "ig_talina",
            position: new mp.Vector3(3309.1604, 5170.56934, 23.5167542),
            heading: -131.919983,
            freezePosition: true,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animName: "li-mi_amb_club_06_base_male^3"
            }
        },
        {
            model: "a_m_y_downtown_01",
            position: new mp.Vector3(3323.01367, 5180.98291, 18.4101143),
            heading: -82.6674271,
            freezePosition: false,
            variations: [{
                    componentId: 1,
                    textureId: 0
                }],
            scenario: "PROP_HUMAN_BBQ"
        },
        {
            model: "a_c_husky",
            position: new mp.Vector3(3319.81567, 5165.6543, 17.7991924),
            heading: 36.8841515,
            freezePosition: true,
            variations: [{
                    componentId: 1,
                    textureId: 0
                }],
            animation: {
                animDict: "creatures@retriever@amb@world_dog_sitting@idle_a",
                animName: "idle_c"
            }
        },
        {
            model: "a_f_m_soucent_01",
            position: new mp.Vector3(3317.62842, 5183.04688, 19.1395645),
            heading: 125.999306,
            freezePosition: true,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@prop_human_seat_chair@female@arms_folded@base",
                animName: "base"
            }
        },
        {
            model: "ig_brucie2",
            position: new mp.Vector3(3334.50806, 5163.6123, 18.8601246),
            heading: 172.036835,
            freezePosition: true,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "mini@repair",
                animName: "fixing_a_player"
            }
        },
        {
            model: "a_c_rabbit_01",
            position: new mp.Vector3(3269.65747, 5211.47559, 18.4818153),
            heading: 134.136063,
            freezePosition: false,
            variations: [{
                    componentId: 4,
                    textureId: 0
                }],
            animation: {
                animDict: "creatures@rabbit@amb@world_rabbit_eating@idle_a",
                animName: "idle_c"
            }
        },
        {
            model: "a_c_deer",
            position: new mp.Vector3(3203.88501, 5229.93311, 24.417963),
            heading: 33.6960068,
            freezePosition: false,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "creatures@deer@amb@world_deer_grazing@idle_a",
                animName: "idle_b"
            }
        },
        {
            model: "a_c_deer",
            position: new mp.Vector3(3200.8103, 5231.96045, 24.7930107),
            heading: -100.823029,
            freezePosition: false,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "creatures@deer@amb@world_deer_grazing@idle_a",
                animName: "idle_a"
            }
        },
        {
            model: "a_c_crow",
            position: new mp.Vector3(3322.33032, 5136.71094, 25.8094921),
            heading: -93.0428467,
            freezePosition: true,
            variations: [{
                    componentId: 2,
                    textureId: 0
                }],
            animation: {
                animDict: "creatures@crow@amb@world_crow_feeding@idle_a",
                animName: "idle_c"
            }
        },
        {
            model: "a_c_cat_01",
            position: new mp.Vector3(3313.90186, 5180.23389, 19.244772),
            heading: 89.9555435,
            freezePosition: true,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "creatures@cat@amb@world_cat_sleeping_ground@idle_a",
                animName: "idle_a"
            }
        },
        {
            model: "ig_djtalaurelia",
            position: new mp.Vector3(3312.91431, 5174.03906, 19.6046715),
            heading: -38.0713577,
            freezePosition: true,
            variations: [{
                    componentId: 0,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@world_human_leaning@female@wall@back@holding_elbow@idle_a",
                animName: "idle_a"
            }
        },
        {
            model: "ig_natalia",
            position: new mp.Vector3(3352.28442, 5151.26563, 19.5803242),
            heading: -86.9393692,
            freezePosition: true,
            variations: [{
                    componentId: 1,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@world_human_seat_steps@female@hands_by_sides@idle_a",
                animName: "idle_b"
            }
        },
        {
            model: "ig_chengsr",
            position: new mp.Vector3(3333.19434, 5163.78418, 18.2827778),
            heading: -128.750259,
            freezePosition: false,
            variations: [{
                    componentId: 5,
                    textureId: 0
                }],
            scenario: "WORLD_HUMAN_SMOKING"
        },
        {
            model: "ig_joeminuteman",
            position: new mp.Vector3(3352.31885, 5150.58545, 19.6126442),
            heading: -93.9699249,
            freezePosition: true,
            variations: [{
                    componentId: 3,
                    textureId: 0
                }],
            animation: {
                animDict: "amb@world_human_seat_steps@male@elbows_on_knees@idle_a",
                animName: "idle_b"
            }
        }
    ],
    vehicles: [
        {
            model: "sadler2",
            position: new mp.Vector3(3333.436, 5160.256, 18.03778),
            rotation: -21.69901,
            colors: [
                111,
                111
            ],
            numberPlate: "6TRJ244",
            engine: false,
            doors: { 4: true }
        },
        {
            model: "asea2",
            position: new mp.Vector3(3329.896, 5148.669, 17.79387),
            rotation: -49.22009,
            colors: [
                111,
                111
            ],
            numberPlate: "6LNE878",
            engine: false
        }
    ],
    deleteObjects: [
        {
            x: 3284.033,
            y: 5183.909,
            z: 17.40463,
            radius: 1,
            model: 1430257647
        },
        {
            x: 3280.056,
            y: 5182.129,
            z: 17.41824,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3279.057,
            y: 5183.562,
            z: 17.42972,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3280.868,
            y: 5185.967,
            z: 17.40671,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3282.968,
            y: 5188.854,
            z: 17.54349,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3285.258,
            y: 5189.526,
            z: 17.4549,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3284.424,
            y: 5190.939,
            z: 17.44394,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3283.412,
            y: 5187.147,
            z: 17.54727,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3281.382,
            y: 5184.354,
            z: 17.49258,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3285.369,
            y: 5178.51,
            z: 17.46924,
            radius: 1,
            model: -634939447
        },
        {
            x: 3287.369,
            y: 5180.144,
            z: 17.55673,
            radius: 1,
            model: -634939447
        },
        {
            x: 3287.635,
            y: 5181.479,
            z: 17.50372,
            radius: 1,
            model: -634939447
        },
        {
            x: 3289.756,
            y: 5183.258,
            z: 17.50137,
            radius: 1,
            model: -634939447
        },
        {
            x: 3290.468,
            y: 5185.189,
            z: 17.44962,
            radius: 1,
            model: -634939447
        },
        {
            x: 3290.935,
            y: 5191.012,
            z: 17.3992,
            radius: 1,
            model: -1572018818
        },
        {
            x: 3292.416,
            y: 5192.964,
            z: 17.37476,
            radius: 1,
            model: -476379988
        },
        {
            x: 3299.876,
            y: 5197.479,
            z: 17.00595,
            radius: 1,
            model: -2129526670
        },
        {
            x: 3295.948,
            y: 5195.787,
            z: 17.57071,
            radius: 1,
            model: -1992580192
        },
        {
            x: 3303.843,
            y: 5187.63,
            z: 17.76636,
            radius: 1,
            model: -130812911
        },
        {
            x: 3318.349,
            y: 5183.484,
            z: 17.42154,
            radius: 1,
            model: 670963709
        },
        {
            x: 3324.576,
            y: 5175.643,
            z: 17.39679,
            radius: 1,
            model: 667168444
        },
        {
            x: 3314.65,
            y: 5181.181,
            z: 19.01733,
            radius: 1,
            model: -380698483
        },
        {
            x: 3313.189,
            y: 5174.06,
            z: 18.60385,
            radius: 1,
            model: -2084538847
        },
        {
            x: 3311.008,
            y: 5175.305,
            z: 20.53372,
            radius: 1,
            model: 123739945
        },
        {
            x: 3311.51,
            y: 5177.44,
            z: 18.58679,
            radius: 1,
            model: 1458701228
        },
        {
            x: 3316.377,
            y: 5183.214,
            z: 18.75513,
            radius: 1,
            model: -199904194
        },
        {
            x: 3332.147,
            y: 5165.676,
            z: 17.45877,
            radius: 1,
            model: -774156031
        },
        {
            x: 3337.13,
            y: 5163.469,
            z: 17.2767,
            radius: 1,
            model: -171729071
        },
        {
            x: 3337.178,
            y: 5161.657,
            z: 17.47769,
            radius: 1,
            model: 212098417
        },
        {
            x: 3335.966,
            y: 5153.887,
            z: 17.29013,
            radius: 1,
            model: 765541575
        },
        {
            x: 3334.546,
            y: 5151.686,
            z: 17.23459,
            radius: 1,
            model: 765541575
        },
        {
            x: 3332.455,
            y: 5149.065,
            z: 17.24597,
            radius: 1,
            model: 1072616162
        },
        {
            x: 3331.057,
            y: 5147.357,
            z: 17.34763,
            radius: 1,
            model: 765541575
        },
        {
            x: 3324.384,
            y: 5168.58,
            z: 17.42737,
            radius: 1,
            model: 731682010
        },
        {
            x: 3333.479,
            y: 5164.159,
            z: 17.31213,
            radius: 1,
            model: -921781850
        },
        {
            x: 3326.957,
            y: 5187.98,
            z: 17.22168,
            radius: 1,
            model: -1685705098
        },
        {
            x: 3288.956,
            y: 5190.982,
            z: 17.40176,
            radius: 1,
            model: -1714859751
        },
        {
            x: 3310.609,
            y: 5159.144,
            z: 17.40176,
            radius: 1,
            model: -157551270
        },
        {
            x: 3303.46,
            y: 5184.926,
            z: 18.71429,
            radius: 1,
            model: 129608276
        },
        {
            x: 3301.497,
            y: 5184.78,
            z: 17.92828,
            radius: 1,
            model: 1270590574
        },
        {
            x: 3306.637,
            y: 5195.023,
            z: 17.42139,
            radius: 1,
            model: -1782242710
        }
    ]
};

export default scenario;