const scenario = {
    isPlaying: false,

    playerPosition: new mp.Vector3(1533.79053, 6623.119, 1.50712311),
	time: {
		hour: 13,
		minute: 10,
		second: 0
	},
    weather: "CLEAR",

    ambientMusic: "intro.beach",
    iplName: "mj_startscreen_summer",

    defaultSceneDuration: 7000,
    cameras: [
		{
			from: {
				pos: new mp.Vector3(1483.96826, 6760.72559, -20.0270805),
				pointAt: new mp.Vector3(1491.64966, 6741.7041, -12.7472305),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .4
				},
				fov: 30,
				dof: {
					nearDof: 1,
					farDof: 30,
					strength: 1,
					shallowMode: !0
				}
			},
			duration: 1e4
		}, {
			from: {
				pos: new mp.Vector3(1493.13013, 6627.9126, 2.44088411),
				pointAt: new mp.Vector3(1495.8418, 6631.29053, 2.600281),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 35,
				dof: {
					nearDof: 1,
					farDof: 50,
					strength: 1,
					shallowMode: !0
				}
			}
		}, {
			from: {
				pos: new mp.Vector3(1545.60327, 6654.479, 6.48325872),
				pointAt: new mp.Vector3(1547.7948, 6649.0332, 1.77074158),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 35,
				dof: {
					nearDof: 1,
					farDof: 10,
					strength: 1,
					shallowMode: !0
				}
			},
			to: {
				pos: new mp.Vector3(1541.30432, 6649.87451, 6.48325872),
				pointAt: new mp.Vector3(1547.7948, 6649.0332, 1.77074158),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 35,
				dof: {
					nearDof: 1,
					farDof: 10,
					strength: 1,
					shallowMode: !0
				}
			},
			duration: 8e3
		}, {
			from: {
				pos: new mp.Vector3(1501.14319, 6634.85352, 1.29143095),
				pointAt: new mp.Vector3(1502.05566, 6635.60693, 1.22276545),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 35,
				dof: {
					nearDof: 1,
					farDof: 10,
					strength: 1,
					shallowMode: !0
				}
			}
		}, {
			from: {
				pos: new mp.Vector3(1562.49084, 6647.36963, 2.98420596),
				pointAt: new mp.Vector3(1560.36816, 6648.51953, 3.01998615),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 40,
				dof: {
					nearDof: 1,
					farDof: 6,
					strength: 1,
					shallowMode: !0
				}
			}
		}, {
			from: {
				pos: new mp.Vector3(1537.1687, 6625.30127, 3.01284862),
				pointAt: new mp.Vector3(1527.56836, 6629.74561, 2.50184679),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 25,
				dof: {
					nearDof: 1,
					farDof: 15,
					strength: 1,
					shallowMode: !0
				}
			},
			to: {
				pos: new mp.Vector3(1533.76953, 6620.39648, 2.51284909),
				pointAt: new mp.Vector3(1527.56836, 6629.74561, 2.50184679),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 25,
				dof: {
					nearDof: 1,
					farDof: 15,
					strength: 1,
					shallowMode: !0
				}
			},
			duration: 8e3
		}, {
			from: {
				pos: new mp.Vector3(1495.86499, 6629.69287, 1.64574385),
				pointAt: new mp.Vector3(1506.56494, 6612.87988, 3.47582316),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 13,
				dof: {
					nearDof: 1,
					farDof: 15,
					strength: 1,
					shallowMode: !0
				}
			}
		}, {
			from: {
				pos: new mp.Vector3(1486.00525, 6600.95264, 15.7759609),
				pointAt: new mp.Vector3(1494.60437, 6613.31494, 14.8240366),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 20,
				dof: {
					nearDof: 1,
					farDof: 60,
					strength: 1,
					shallowMode: !0
				}
			}
		}, {
			from: {
				pos: new mp.Vector3(1486.57886, 6630.11328, 3.38183713),
				pointAt: new mp.Vector3(1482.2948, 6644.44043, 3.43600988),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 20,
				dof: {
					nearDof: 1,
					farDof: 60,
					strength: 1,
					shallowMode: !0
				}
			},
			to: {
				pos: new mp.Vector3(1486.57886, 6630.11328, 2.33183813),
				pointAt: new mp.Vector3(1482.2948, 6644.44043, 2.34601092),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 20,
				dof: {
					nearDof: 1,
					farDof: 60,
					strength: 1,
					shallowMode: !0
				}
			},
			duration: 8e3
		}, {
			from: {
				pos: new mp.Vector3(1541.00317, 6637.23828, 1.46332622),
				pointAt: new mp.Vector3(1543.76428, 6638.37988, 1.79957092),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 19,
				dof: {
					nearDof: 1,
					farDof: 7,
					strength: 1,
					shallowMode: !0
				}
			}
		}, {
			from: {
				pos: new mp.Vector3(1538.02539, 6635.9375, 1.51807857),
				pointAt: new mp.Vector3(1532.81177, 6649.896, .67993027),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 12,
				dof: {
					nearDof: 1,
					farDof: 6,
					strength: 1,
					shallowMode: !0
				}
			}
		}, {
			from: {
				pos: new mp.Vector3(1557.78821, 6661.04785, 3.26499271),
				pointAt: new mp.Vector3(1565.67249, 6662.54443, 1.24814212),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 25,
				dof: {
					nearDof: 1,
					farDof: 10,
					strength: 1,
					shallowMode: !0
				}
			},
			to: {
				pos: new mp.Vector3(1557.78821, 6661.04785, 3.26499271),
				pointAt: new mp.Vector3(1584.05469, 6700.53516, .103681572),
				shake: {
					type: "HAND_SHAKE",
					amplitude: .2
				},
				fov: 25,
				dof: {
					nearDof: 1,
					farDof: 30,
					strength: 1,
					shallowMode: !0
				}
			},
			duration: 8e3
		}
	],
    peds: [ {
		model: "a_f_m_beach_01",
		position: new mp.Vector3(1548.7356, 6646.60254, 1.87852359),
		rotation: new mp.Vector3(-4.70000124, 2.14163549e-7, 45.805603),
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
		model: "a_m_m_soucent_03",
		position: new mp.Vector3(1559.9187, 6648.26562, 2.38473654),
		rotation: new mp.Vector3(0, 0, -84.2144165),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@prop_human_seat_bar@male@hands_on_bar@idle_a",
			animName: "idle_c"
		}
	}, {
		model: "g_f_y_ballas_01",
		position: new mp.Vector3(1560.20813, 6649.74609, 2.61748791),
		rotation: new mp.Vector3(0, 0, -95.7610703),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@casino@brawl@reacts@bar@",
			animName: "f_bar_01_gawk_loop_03"
		}
	}, {
		model: "a_f_y_business_04",
		position: new mp.Vector3(1561.81519, 6648.35986, 2.50145664),
		rotation: new mp.Vector3(0, 0, 89.4556274),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@clubhouse@bar@drink@base",
			animName: "idle_a"
		}
	}, {
		model: "a_f_y_bevhills_04",
		position: new mp.Vector3(1550.94446, 6620.94482, 1.95577824),
		rotation: new mp.Vector3(0, 0, 81.3665466),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@beach_party@",
			animName: "seated_female_a_idle_d"
		}
	}, {
		model: "a_f_y_juggalo_01",
		position: new mp.Vector3(1530.52588, 6623.55664, 2.54418349),
		rotation: new mp.Vector3(0, 0, -53.5772705),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@nightclub_island@dancers@crowddance_groups@groupe@",
			animName: "hi_dance_crowd_11_v2_female^2"
		}
	}, {
		model: "a_f_y_beach_01",
		position: new mp.Vector3(1547.06226, 6646.88623, 1.78640342),
		rotation: new mp.Vector3(-4.20000172, 2.14018115e-7, 42.9840393),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@prop_human_seat_sunlounger@female@idle_a",
			animName: "idle_a"
		}
	}, {
		model: "a_f_y_clubcust_02",
		position: new mp.Vector3(1533.85229, 6627.1875, 2.51726508),
		rotation: new mp.Vector3(0, 0, 49.1267357),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@nightclub_island@dancers@crowddance_facedj@hi_intensity",
			animName: "hi_dance_facedj_hu_15_v1_female^3"
		}
	}, {
		model: "a_m_m_salton_03",
		position: new mp.Vector3(1532.85889, 6623.97852, 2.52177405),
		rotation: new mp.Vector3(0, 0, 64.5840988),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity",
			animName: "hi_dance_facedj_17_v1_male^3"
		}
	}, {
		model: "a_m_m_beach_01",
		position: new mp.Vector3(1501.23718, 6618.23975, 2.50197721),
		rotation: new mp.Vector3(0, 0, -34.9875374),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@prop_human_muscle_chin_ups@male@base",
			animName: "base"
		}
	}, {
		model: "a_m_y_vinewood_02",
		position: new mp.Vector3(1527.04517, 6630.33936, 2.6085403),
		rotation: new mp.Vector3(0, 0, -143.512573),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@nightclub@djs@solomun@",
			animName: "temp_slmn_set_solomun"
		}
	}, {
		model: "a_f_y_hippie_01",
		position: new mp.Vector3(1512.75415, 6631.93994, 2.13129926),
		rotation: new mp.Vector3(-4.89999819, 2.14226276e-7, -48.330822),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "savecouch@",
			animName: "t_sleep_loop_couch"
		}
	}, {
		model: "a_f_y_tourist_01",
		position: new mp.Vector3(1509.38623, 6631.04199, 2.59243703),
		rotation: new mp.Vector3(0, 0, 3.57143688),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@business@bgen@bgen_no_work@",
			animName: "sit_phone_idle_01-noworkfemale"
		}
	}, {
		model: "a_m_m_rurmeth_01",
		position: new mp.Vector3(1561.67151, 6661.37549, 1.65883613),
		rotation: new mp.Vector3(27.9999771, -3.19999886, 24.0067596),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@world_human_seat_steps@male@hands_in_lap@idle_b",
			animName: "idle_d"
		}
	}, {
		model: "a_f_y_topless_01",
		position: new mp.Vector3(1486.052, 6633.28955, 1.90834641),
		rotation: new mp.Vector3(0, 0, 19.1409988),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@world_human_seat_steps@female@hands_by_sides@idle_b",
			animName: "idle_f"
		}
	}, {
		model: "a_m_y_salton_01",
		position: new mp.Vector3(1553.33386, 6623.53418, 2.37230349),
		rotation: new mp.Vector3(368484743e-24, 309777816e-24, 44.5316658),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		scenario: "PROP_HUMAN_BBQ"
	}, {
		model: "a_m_y_jetski_01",
		position: new mp.Vector3(1544.37109, 6638.92578, 2.40327525),
		rotation: new mp.Vector3(0, 0, -35.9179497),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@garage@chassis_repair@",
			animName: "look_around_01_amy_skater_01"
		}
	}, {
		model: "a_m_y_surfer_01",
		position: new mp.Vector3(1497.38159, 6630.98535, 2.56662226),
		rotation: new mp.Vector3(0, 0, 79.219902),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@casino@brawl@fights@guard@",
			animName: "argument_loop_mp_m_brawler_02"
		}
	}, {
		model: "a_m_y_jetski_01",
		position: new mp.Vector3(1543.1947, 6657.40234, -.322343141),
		rotation: new mp.Vector3(20.8455372, 0, 43.8888435),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@clubhouse@seating@male@var_a@base@",
			animName: "idle_b"
		}
	}, {
		model: "s_f_y_shop_low",
		position: new mp.Vector3(1547.43188, 6637.39355, 2.43003607),
		rotation: new mp.Vector3(0, 0, -39.2043915),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@world_human_picnic@female@idle_a",
			animName: "idle_b"
		}
	}, {
		model: "a_m_y_breakdance_01",
		position: new mp.Vector3(1503.86548, 6618.18799, 2.43877125),
		rotation: new mp.Vector3(6.25322405e-10, 155300513e-26, 88.2068863),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@beach_party@",
			animName: "lean_male_a_idle_b"
		}
	}, {
		model: "s_f_y_baywatch_01",
		position: new mp.Vector3(1496.09009, 6631.26221, 2.54332852),
		rotation: new mp.Vector3(0, 0, -116.400871),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@beach_party@",
			animName: "lean_female_a_idle_c"
		}
	}, {
		model: "u_f_y_lauren",
		position: new mp.Vector3(1548.94519, 6651.25049, 2.73291612),
		rotation: new mp.Vector3(-135536557e-13, 1.99999833, 144.816757),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "timetable@tracy@sleep@",
			animName: "idle_c"
		}
	}, {
		model: "a_m_y_stwhi_01",
		position: new mp.Vector3(1550.10339, 6634.14551, 2.20641446),
		rotation: new mp.Vector3(0, 0, 31.7974186),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@world_human_seat_steps@male@hands_in_lap@idle_a",
			animName: "idle_b"
		}
	}, {
		model: "a_m_y_musclbeac_02",
		position: new mp.Vector3(1530.80627, 6625.62891, 2.54361105),
		rotation: new mp.Vector3(0, 0, -119.756653),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@nightclub_island@dancers@crowddance_facedj@hi_intensity",
			animName: "hi_dance_facedj_hu_15_v1_male^6"
		}
	}, {
		model: "a_m_y_polynesian_01",
		position: new mp.Vector3(1546.81848, 6621.21143, 1.90268731),
		rotation: new mp.Vector3(0, 0, -126.127991),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@beach_party@",
			animName: "seated_male_a_idle_d"
		}
	}, {
		model: "a_c_husky",
		position: new mp.Vector3(1548.23865, 6638.81396, 1.89050555),
		rotation: new mp.Vector3(-2.18919873, -1.06799625e-7, 143.938812),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "creatures@rottweiler@amb@world_dog_sitting@idle_a",
			animName: "idle_c"
		}
	}, {
		model: "a_c_dolphin",
		position: new mp.Vector3(1491.36182, 6741.11133, -14.0295181),
		rotation: new mp.Vector3(23.0701275, 9.27988594e-7, 133.551895),
		freezePosition: !1,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "creatures@dolphin@move",
			animName: "swim_turn_r"
		}
	}, {
		model: "a_c_killerwhale",
		position: new mp.Vector3(1487.0011, 6741.30371, -12.3238207),
		rotation: new mp.Vector3(0, -0, -126.236549),
		freezePosition: !1,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "creatures@killerwhale@move",
			animName: "idle_turn_l"
		}
	}, {
		model: "g_f_y_vagos_01",
		position: new mp.Vector3(1533.0675, 6628.58301, 2.51810122),
		rotation: new mp.Vector3(0, -0, 141.438629),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_hi_intensity",
			animName: "trans_dance_facedj_hi_to_li_07_v1_female^2"
		}
	}, {
		model: "a_c_rhesus",
		position: new mp.Vector3(1487.9965, 6603.1338, 15.312),
		rotation: new mp.Vector3(-9.18587685, 2.16216264e-7, 109.050674),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "amb@prop_human_seat_sunlounger@male@idle_a",
			animName: "idle_d"
		}
	}, {
		model: "a_c_seagull",
		position: new mp.Vector3(1536.2782, 6639.65088, 1.106969),
		rotation: new mp.Vector3(-0, -0, -78.4371262),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "creatures@gull@amb@world_gull_standing@idle_a",
			animName: "idle_b"
		}
	}, {
		model: "a_c_seagull",
		position: new mp.Vector3(1537.00854, 6640.12598, 1.26923108),
		rotation: new mp.Vector3(-40989421e-13, -508888789e-22, 170.163574),
		freezePosition: !0,
		variations: [ {
			componentId: 0,
			textureId: 0
		} ],
		animation: {
			animDict: "creatures@gull@amb@world_gull_standing@idle_a",
			animName: "idle_a"
		}
	} ],

    vehicles: [ {
		model: "tropic2",
		position: new mp.Vector3(1436.7189, 6648.2778, 11.9355),
		rotation: new mp.Vector3(0, 0, 177.6),
		colors: [ 0, 28 ],
		freezePosition: !1,
		lights: 2,
		numberPlate: "LENDOS",
		engine: !1,
		dirt: 0,
		doors: {
			5: !0
		},
		tuning: {
			48: 6
		}
	}, {
		model: "tropic2",
		position: new mp.Vector3(1485.0243, 6700.1987, 1.1255),
		rotation: new mp.Vector3(0, 0, 149.6),
		colors: [ 111, 0 ],
		freezePosition: !0,
		lights: 2,
		numberPlate: "LENDOS",
		engine: !1,
		dirt: 0,
		doors: {
			5: !0
		},
		tuning: {
			48: 2
		}
	} ],

    deleteObjects: [{
		x: 1606.87793,
		y: 6618.889,
		z: 14.9125671,
		radius: 1,
		model: 2722948478
	}, {
		x: 1606.26465,
		y: 6629.21143,
		z: 14.4136047,
		radius: 1,
		model: 2722948478
	}, {
		x: 1600.08789,
		y: 6632.476,
		z: 14.3852844,
		radius: 1,
		model: 2722948478
	}, {
		x: 1604.57837,
		y: 6611.596,
		z: 15.0221863,
		radius: 1,
		model: 2722948478
	} ],
};

export default scenario;