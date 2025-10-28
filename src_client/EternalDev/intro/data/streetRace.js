const scenario = {
    isPlaying: false,

    playerPosition: new mp.Vector3(385.223755, -752.47345, 26.4760361),
    time: {
        hour: 1,
        minute: 10,
        second: 0
    },
    weather: "CLEAR",

    ambientMusic: "intro.streetrace",
    iplName: "mj_startscreen_spring",

	scenes: {
        auth: {
            type: "interpolate",
            camPos: new mp.Vector3(377.4114, -753.2484, 29.645),
            camLookAt: new mp.Vector3(376.72, -754.9167, 29.67197),
            fov: 40,
            dof: {
                nearDof: 1,
                farDof: 2,
                strength: 1,
                shallowMode: true
            }
        },
        characters: {
            type: "interpolate",
            camPos: new mp.Vector3(370.0362, -756.822, 33.37207),
            camLookAt: new mp.Vector3(368.2539, -764.4686, 28.28547),
            fov: 45,
            dof: {
                nearDof: 1,
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
            position: new mp.Vector3(370.075684, -766.240051, 29.3126526),
            heading: -80.7024155,
            camPos: new mp.Vector3(371.15, -767.26, 29.52),
            camLookAt: new mp.Vector3(370.483, -766.0106, 29.43605),
            fov: 50,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: true
            },
            freezePosition: true,
            animation: {
                animDict: "anim@amb@warehouse@laptop@",
                animName: "idle_a"
            }
        },
        2: {
            position: new mp.Vector3(365.67691, -768.244446, 29.1485786),
            heading: -18.0355701,
            camPos: new mp.Vector3(365.0466, -766.2886, 29.475),
            camLookAt: new mp.Vector3(366.1187, -767.793, 29.39),
            fov: 45,
            dof: {
                nearDof: 0,
                farDof: 2,
                strength: 1,
                shallowMode: true
            },
            freezePosition: true,
            animation: {
                animDict: "amb@world_human_seat_steps@male@elbows_on_knees@idle_a",
                animName: "idle_a"
            }
        },
        3: {
            position: new mp.Vector3(367.111816, -764.106628, 29.2765961),
            heading: -132.325516,
            camPos: new mp.Vector3(367.6421, -765.9658, 29.40998),
            camLookAt: new mp.Vector3(367.2136, -764.335, 29.35461),
            fov: 45,
            dof: {
                nearDof: 0,
                farDof: 3,
                strength: 1,
                shallowMode: true
            },
            freezePosition: true,
            animation: {
                animDict: "amb@world_human_leaning@male@wall@back@legs_crossed@idle_a",
                animName: "idle_b"
            }
        }
    },

    defaultSceneDuration: 7000,
    cameras: [
		{
			from: {
				pos: new mp.Vector3(377.377, -735.7278, 30.96309),
				pointAt: new mp.Vector3(380.9797, -749.7139, 28.29322),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.4
				},
				fov: 45,
				dof: {
					nearDof: 1,
					farDof: 30,
					strength: 1,
					shallowMode: true
				}
			},
			duration: 10000
		},
		{
			from: {
				pos: new mp.Vector3(385.2976, -748.5569, 28.96871),
				pointAt: new mp.Vector3(386.5763, -752.4672, 28.8455),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.4
				},
				fov: 45,
				dof: {
					nearDof: 0,
					farDof: 7,
					strength: 1,
					shallowMode: true
				}
			},
			to: {
				pos: new mp.Vector3(384.0747, -749.8177, 28.96871),
				pointAt: new mp.Vector3(386.5763, -752.4672, 28.8455),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.4
				},
				fov: 45,
				dof: {
					nearDof: 0,
					farDof: 7,
					strength: 1,
					shallowMode: true
				}
			},
			duration: 8000
		},
		{
			from: {
				pos: new mp.Vector3(380.3275, -750.2731, 29.09298),
				pointAt: new mp.Vector3(378.9577, -752.2989, 29.1176),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.2
				},
				fov: 40,
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
				pos: new mp.Vector3(378.4597, -763.6951, 29.21956),
				pointAt: new mp.Vector3(377.0826, -765.6047, 29.12883),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.2
				},
				fov: 45,
				dof: {
					nearDof: 1,
					farDof: 5,
					strength: 1,
					shallowMode: true
				}
			}
		},
		{
			from: {
				pos: new mp.Vector3(381.5022, -744.9899, 29.41807),
				pointAt: new mp.Vector3(379.0538, -743.9249, 29.23175),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.6
				},
				fov: 43,
				dof: {
					nearDof: 0,
					farDof: 12,
					strength: 1,
					shallowMode: true
				}
			},
			to: {
				pos: new mp.Vector3(384.4003, -745.272, 29.41807),
				pointAt: new mp.Vector3(375.356, -748.177, 29.32811),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.7
				},
				fov: 43,
				dof: {
					nearDof: 0,
					farDof: 12,
					strength: 1,
					shallowMode: true
				}
			},
			duration: 8000
		},
		{
			from: {
				pos: new mp.Vector3(388.1834, -768.0557, 29.25917),
				pointAt: new mp.Vector3(387.0593, -766.274, 29.16228),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.3
				},
				fov: 45,
				dof: {
					nearDof: 0,
					farDof: 15,
					strength: 1,
					shallowMode: true
				}
			}
		},
		{
			from: {
				pos: new mp.Vector3(382.7247, -770.4851, 29.1062),
				pointAt: new mp.Vector3(383.4579, -768.8051, 29.21866),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.3
				},
				fov: 40,
				dof: {
					nearDof: 0,
					farDof: 6,
					strength: 1,
					shallowMode: true
				}
			}
		},
		{
			from: {
				pos: new mp.Vector3(393.2626, -743.986, 28.9729),
				pointAt: new mp.Vector3(392.1345, -742.7717, 28.97658),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.4
				},
				fov: 40,
				dof: {
					nearDof: 0,
					farDof: 5,
					strength: 1,
					shallowMode: true
				}
			},
			to: {
				pos: new mp.Vector3(392.1189, -744.3188, 28.9729),
				pointAt: new mp.Vector3(391.5744, -743.0128, 28.97133),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.4
				},
				fov: 40,
				dof: {
					nearDof: 0,
					farDof: 5,
					strength: 1,
					shallowMode: true
				}
			},
			duration: 8000
		},
		{
			from: {
				pos: new mp.Vector3(387.5726, -733.6839, 29.58487),
				pointAt: new mp.Vector3(387.551, -734.8198, 29.62854),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.3
				},
				fov: 47,
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
				pos: new mp.Vector3(386.1906, -774.1548, 39.84564),
				pointAt: new mp.Vector3(386.0655, -773.2357, 39.46035),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.2
				},
				fov: 42,
				dof: {
					nearDof: 0,
					farDof: 40,
					strength: 1,
					shallowMode: true
				}
			}
		},
		{
			from: {
				pos: new mp.Vector3(402.0863, -745.0959, 29.26962),
				pointAt: new mp.Vector3(394.05, -756.7403, 28.99414),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.4
				},
				fov: 40,
				dof: {
					nearDof: 0,
					farDof: 40,
					strength: 1,
					shallowMode: true
				}
			},
			to: {
				pos: new mp.Vector3(402.0863, -745.0959, 29.26962),
				pointAt: new mp.Vector3(390.9113, -754.5637, 28.99414),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.4
				},
				fov: 38,
				dof: {
					nearDof: 0,
					farDof: 40,
					strength: 1,
					shallowMode: true
				}
			},
			duration: 8000
		},
		{
			from: {
				pos: new mp.Vector3(375.26, -743.4791, 30.09448),
				pointAt: new mp.Vector3(375.8797, -744.1419, 30.11174),
				shake: {
					type: "HAND_SHAKE",
					amplitude: 0.3
				},
				fov: 50,
				dof: {
					nearDof: 0,
					farDof: 3,
					strength: 1,
					shallowMode: true
				}
			}
		}
	],
    peds: [
        {
			model: "mp_m_waremech_01",
			position: new mp.Vector3(375.833344, -750.473755, 29.3569317),
			heading: -14.1179447,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@warehouse@toolbox@",
				animName: "idle"
			}
		},
		{
			model: "g_m_y_azteca_01",
			position: new mp.Vector3(382.921631, -746.786072, 29.2888088),
			heading: 13.9447088,
			freezePosition: false,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@nightclub@dancers@crowddance_facedj@",
				animName: "hi_dance_facedj_09_v1_male^5"
			}
		},
		{
			model: "g_m_y_armgoon_02",
			position: new mp.Vector3(381.211029, -745.451416, 29.2994308),
			heading: -130.78714,
			freezePosition: false,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@nightclub_island@dancers@crowddance_facedj@",
				animName: "hi_dance_facedj_09_v1_male^3"
			}
		},
		{
			model: "ig_g",
			position: new mp.Vector3(386.578705, -773.186829, 39.1333656),
			heading: 38.1464691,
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
			model: "ig_jimmydisanto",
			position: new mp.Vector3(381.563049, -767.375488, 29.124464),
			heading: 89.2119217,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "switch@franklin@bed",
				animName: "bed_reading_loop"
			}
		},
		{
			model: "ig_hao",
			position: new mp.Vector3(375.720337, -761.51416, 29.2975616),
			heading: 109.77182,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@scripted@submarine@special_peds@pavel@hs4_pavel_ig1_screens",
				animName: "base_idle"
			}
		},
		{
			model: "ig_stretch",
			position: new mp.Vector3(384.927551, -753.143372, 28.7136803),
			heading: -8.89052868,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "amb@world_human_seat_steps@male@hands_in_lap@idle_b",
				animName: "idle_d"
			}
		},
		{
			model: "ig_sol",
			position: new mp.Vector3(385.874603, -752.231567, 29.2961349),
			heading: 101.471817,
			freezePosition: false,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			scenario: "WORLD_HUMAN_SMOKING"
		},
		{
			model: "u_m_m_jesus_01",
			position: new mp.Vector3(383.979584, -742.300659, 29.2979431),
			heading: -6.37612915,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@board_room@diagram_blueprints@",
				animName: "base_amy_skater_01"
			}
		},
		{
			model: "ig_kerrymcintosh",
			position: new mp.Vector3(378.332001, -743.662354, 29.2526526),
			heading: -110.376442,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@nightclub@djs@black_madonna@",
				animName: "temp_blkmdna_set_blackmadonna"
			}
		},
		{
			model: "s_f_y_bartender_01",
			position: new mp.Vector3(382.35672, -744.737732, 29.2939701),
			heading: -177.676468,
			freezePosition: false,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@nightclub@dancers@crowddance_groups@hi_intensity",
				animName: "hi_dance_crowd_09_v1_female^2"
			}
		},
		{
			model: "s_f_y_clubbar_01",
			position: new mp.Vector3(382.078339, -746.638733, 29.2945805),
			heading: -17.0082245,
			freezePosition: false,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@nightclub@dancers@crowddance_facedj@",
				animName: "hi_dance_facedj_09_v1_female^5"
			}
		},
		{
			model: "a_f_y_vinewood_02",
			position: new mp.Vector3(381.468536, -769.003723, 28.8044434),
			heading: 45.0387268,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "amb@prop_human_seat_chair_mp@female@heels@idle_b",
				animName: "idle_e"
			}
		},
		{
			model: "ig_paige",
			position: new mp.Vector3(388.1091, -766.854187, 29.7517872),
			heading: 86.4402313,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "anim@amb@business@bgen@bgen_no_work@",
				animName: "sit_phone_phoneputdown_sleeping-noworkfemale"
			}
		},
		{
			model: "u_f_y_bikerchic",
			position: new mp.Vector3(387.239319, -735.206177, 29.2523479),
			heading: 127.113464,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "misschinese2_crystalmazemcs1_ig",
				animName: "bar_peds_action_janet"
			}
		},
		{
			model: "g_f_y_families_01",
			position: new mp.Vector3(376.845032, -765.207275, 29.0629921),
			heading: 14.0541124,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "amb@world_human_seat_steps@female@hands_by_sides@idle_a",
				animName: "idle_b"
			}
		},
		{
			model: "ig_russiandrunk",
			position: new mp.Vector3(379.013336, -752.542603, 29.2972641),
			heading: 77.968338,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "mini@repair",
				animName: "fixing_a_player"
			}
		},
		{
			model: "u_m_y_sbike",
			position: new mp.Vector3(389.367645, -739.203064, 29.2988548),
			heading: 107.620529,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "amb@world_human_leaning@male@wall@back@foot_up@idle_a",
				animName: "idle_a"
			}
		},
		{
			model: "u_m_y_party_01",
			position: new mp.Vector3(391.176788, -760.971252, 28.8080997),
			heading: 82.8496628,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "amb@prop_human_seat_chair@female@arms_folded@idle_a",
				animName: "idle_a"
			}
		},
		{
			model: "ig_tylerdix",
			position: new mp.Vector3(375.871948, -744.391785, 29.9363499),
			heading: 161.554749,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "misstattoo_parlour@shop_ig_4",
				animName: "customer_loop"
			}
		},
		{
			model: "g_f_y_ballas_01",
			position: new mp.Vector3(375.792847, -745.591858, 29.923439),
			heading: 9.8265543,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "random@shop_tattoo",
				animName: "_idle_c"
			}
		},
		{
			model: "g_f_y_vagos_01",
			position: new mp.Vector3(377.042969, -737.39917, 30.4323769),
			heading: -144.140076,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "amb@world_human_seat_steps@female@hands_by_sides@idle_b",
				animName: "idle_f"
			}
		},
		{
			model: "ig_talcc",
			position: new mp.Vector3(379.111053, -768.750488, 28.8235931),
			heading: -84.8498764,
			freezePosition: true,
			variations: [{
					componentId: 0,
					textureId: 0
				}],
			animation: {
				animDict: "amb@prop_human_seat_chair@male@left_elbow_on_knee@idle_a",
				animName: "idle_c"
			}
		}
	],

    vehicles: [
		{
			model: "g63",
			position: new mp.Vector3(384.219, -739.7361, 28.8383),
			rotation: -4.45112,
			colors: [
				12,
				42
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "LENDOS",
			engine: false,
			doors: { 5: true },
			tuning: {
				0: 8,
				1: 14,
				2: 11,
				3: 15,
				4: 9,
				5: 8,
				6: 19,
				7: 10,
				8: 6,
				10: 2,
				37: 16,
				42: 5,
				44: 0,
				47: 3,
				48: 58
			}
		},
		{
			model: "gt63s",
			position: new mp.Vector3(385.1269, -753.7559, 28.93059),
			rotation: 92.47385,
			colors: [
				0,
				42
			],
			freezePosition: true,
			lights: 2,
			numberPlate: "KILL4",
			engine: true,
			doors: { 1: true },
			tuning: {
				0: 14,
				1: 14,
				2: 17,
				3: 8,
				4: 7,
				5: 3,
				6: 1,
				7: 5,
				8: 0,
				9: 4,
				10: 1,
				37: 1,
				42: 1,
				43: 1,
				44: 1,
				47: 2,
				48: 61
			}
		},
		{
			model: "camaro2",
			position: new mp.Vector3(376.6736, -766.9799, 28.45859),
			rotation: -10.07295,
			colors: [
				42,
				28
			],
			freezePosition: true,
			lights: 2,
			numberPlate: "B4MBLB",
			engine: false,
			tuning: {
				0: 2,
				1: 5,
				2: 1,
				3: 2,
				4: 1,
				5: 1,
				6: 5,
				7: 11,
				8: 2,
				9: 1,
				42: 2,
				44: 0,
				46: 2,
				47: 2,
				48: 9
			}
		},
		{
			model: "e63s",
			position: new mp.Vector3(387.8112, -767.0849, 28.85085),
			rotation: 5.276103,
			colors: [
				0,
				35
			],
			freezePosition: true,
			lights: 2,
			numberPlate: "ECL4S5",
			engine: false,
			tuning: {
				0: 8,
				1: 8,
				2: 4,
				3: 1,
				4: 2,
				5: 1,
				7: 1,
				8: 2,
				9: 4,
				27: 2,
				32: 0,
				33: 6,
				37: 1,
				44: 0,
				46: 2,
				47: 2,
				48: 51
			}
		},
		{
			model: "m3e46",
			position: new mp.Vector3(376.5809, -751.9976, 28.6635),
			rotation: -100.9058,
			colors: [
				63,
				28
			],
			freezePosition: true,
			lights: 2,
			numberPlate: "MSTWNDT",
			engine: true,
			doors: { 4: true },
			tuning: {
				0: 9,
				1: 0,
				2: 3,
				4: 1,
				5: 1,
				6: 1,
				7: 4,
				8: 1,
				9: 1,
				10: 1,
				42: 1,
				43: 2,
				44: 1,
				47: 0,
				48: 32
			}
		},
		{
			model: "supragr",
			position: new mp.Vector3(391.7046, -741.0031, 28.66117),
			rotation: 165.3623,
			colors: [
				111,
				0
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "A80THBST",
			engine: false,
			tuning: {
				0: 1,
				1: 0,
				2: 0,
				4: 0,
				7: 0,
				15: 3,
				23: 245,
				48: 10
			}
		},
		{
			model: "cayenne2",
			position: new mp.Vector3(384.3304, -766.7307, 28.69851),
			rotation: 175.3848,
			colors: [
				22,
				28
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "R1MU5",
			engine: false,
			tuning: {
				0: 4,
				1: 2,
				2: 3,
				3: 2,
				4: 2,
				5: 2,
				6: 3,
				7: 2,
				8: 9,
				9: 0,
				10: 3,
				15: 3,
				42: 3,
				44: 1,
				47: 2,
				48: 32
			}
		},
		{
			model: "nisgtr",
			position: new mp.Vector3(380.4236, -740.4861, 28.60998),
			rotation: -173.6153,
			colors: [
				35,
				0
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "P0UL4L",
			engine: false,
			tuning: {
				0: 3,
				1: 2,
				2: 1,
				3: 2,
				15: 3,
				48: 4
			}
		},
		{
			model: "m850",
			position: new mp.Vector3(387.5463, -740.5361, 28.83366),
			rotation: -174.0692,
			colors: [
				61,
				0
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "K008KB",
			engine: false,
			tuning: {
				0: 3,
				3: 0,
				15: 3
			}
		},
		{
			model: "s15",
			position: new mp.Vector3(391.2956, -766.3629, 28.87113),
			rotation: 7.494022,
			colors: [
				111,
				28
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "51IV4",
			engine: false,
			tuning: {
				0: 5,
				1: 2,
				2: 1,
				3: 1,
				6: 0,
				7: 0,
				15: 3,
				42: 0,
				47: 0,
				48: 9
			}
		},
		{
			model: "taycan",
			position: new mp.Vector3(387.3287, -750.9032, 28.9779),
			rotation: 88.14371,
			colors: [
				111,
				28
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "TM45K",
			engine: false,
			tuning: {
				0: 2,
				1: 10,
				2: 10,
				3: 14,
				7: 3,
				9: 3,
				15: 3,
				42: 3,
				47: 3,
				48: 42
			}
		},
		{
			model: "z800",
			position: new mp.Vector3(389.923, -739.1774, 28.80474),
			rotation: -2.280946,
			colors: [
				0,
				28
			],
			freezePosition: false,
			lights: 2,
			numberPlate: "TTHEM00N",
			engine: false,
			tuning: {
				0: 1,
				1: 0,
				2: 0,
				4: 0,
				5: 4,
				6: 0,
				8: 0,
				45: 0,
				47: 0,
				48: 45
			}
		},
		{
			model: "police4",
			position: new mp.Vector3(401.657, -745.0707, 28.78043),
			rotation: 177.3311,
			colors: [
				10,
				28
			],
			freezePosition: true,
			lights: 2,
			numberPlate: "TS4SG56K",
			engine: false
		},
		{
			model: "veyron",
			position: new mp.Vector3(366.7477, -762.5488, 28.74479),
			rotation: 150.0305,
			colors: [
				159,
				28
			],
			freezePosition: true,
			lights: 2,
			numberPlate: "H4NDS0ME",
			engine: false,
			tuning: {
				1: 9,
				3: 2,
				4: 34,
				5: 2,
				6: 11,
				8: 3,
				10: 30,
				27: 2,
				28: 0,
				29: 1,
				33: 2,
				37: 3,
				40: 3,
				42: 11,
				44: 5,
				45: 4,
				46: 4,
				47: 4,
				48: 19
			}
		},
		{
			model: "brutale",
			position: new mp.Vector3(370.5275, -763.7045, 28.8109),
			rotation: 77.93977,
			colors: [
				28,
				42
			],
			freezePosition: true,
			lights: 2,
			numberPlate: "W1NN3R",
			engine: false
		},
		{
			model: "g63",
			position: new mp.Vector3(365.3607, -768.5478, 28.84283),
			rotation: -114.1652,
			colors: [
				111,
				42
			],
			numberPlate: "JENGAS",
			engine: false,
			freezePosition: true,
			lights: 2,
			doors: { 0: true },
			tuning: {
				0: 3,
				1: 11,
				2: 8,
				3: 12,
				4: 6,
				6: 13,
				7: 6,
				8: 2,
				10: 2,
				31: 0,
				32: 1,
				33: 3,
				37: 11,
				42: 2,
				44: 0,
				47: 3,
				48: 54
			}
		}
	],

    deleteObjects: [
       {
			x: 394.1849,
			y: -739.028,
			z: 28.30836,
			radius: 1,
			model: 666561306
		},
		{
			x: 394.0705,
			y: -737.3533,
			z: 28.74813,
			radius: 1,
			model: -1738103333
		},
		{
			x: 394.1179,
			y: -735.2967,
			z: 28.27756,
			radius: 1,
			model: -1853453107
		}
    ],
};

export default scenario;