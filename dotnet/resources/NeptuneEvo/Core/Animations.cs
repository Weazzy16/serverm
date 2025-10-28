using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Core
{
    class Animations : Script
    {
        internal class Animation
        {
            public string Dictionary { get; }
            public string Name { get; }
            public int Flag { get; }

            public Animation(string dict, string name, int flag)
            {
                Dictionary = dict;
                Name = name;
                Flag = flag;
            }
        }
        private static readonly nLog Log = new nLog("Core.Animations");
        private static IReadOnlyDictionary<string, Animation> AnimList = new Dictionary<string, Animation>()
        {
{"1_1", new Animation("random@mugging3", "handsup_standing_base", 49)},
{"1_2", new Animation("anim@move_hostages@male", "male_idle", 1)},
{"1_3", new Animation("anim@mp_player_intincarsalutestd@ds@", "idle_a", 49)},
{"1_4", new Animation("mp_player_int_uppersalute", "mp_player_int_salute", 49)},
{"1_5", new Animation("mp_player_intsalute", "mp_player_int_salute", 1)},
{"1_6", new Animation("anim@gangops@morgue@table@", "player_search", 49)},
{"1_7", new Animation("taxi_hail", "fp_hail_taxi", 48)},
{"1_8", new Animation("amb@world_human_stand_fire@male@base", "base", 1)},
{"1_9", new Animation("move_m@drunk@transitions", "slightly_to_idle", 1)},
{"1_10", new Animation("amb@world_human_sit_ups@male@base", "base", 1)},
{"1_11", new Animation("amb@world_human_bum_wash@male@high@idle_a", "idle_a", 1)},
{"1_12", new Animation("random@car_thief@victimpoints_ig_3", "arms_waving", 1)},
{"1_13", new Animation("amb@medic@standing@kneel@enter", "enter", 1)},
{"1_14", new Animation("amb@code_human_police_investigate@idle_b", "idle_f", 1)},
{"1_15", new Animation("timetable@reunited@ig_2", "jimmy_base", 1)},
{"1_16", new Animation("amb@world_human_push_ups@male@base", "base", 1)},
{"1_17", new Animation("anim@mp_freemode_return@f@fail", "fail_a", 1)},
{"1_18", new Animation("amb@code_human_police_investigate@idle_a", "idle_a", 1)},
{"1_19", new Animation("gestures@m@standing@casual", "gesture_what_soft", 0)},
{"1_20", new Animation("amb@code_human_in_car_mp_actions@v_sign@bodhi@rps@base", "idle_a", 49)},
{"1_21", new Animation("gestures@m@standing@casual", "gesture_point", 48)},
{"1_22", new Animation("mp_player_int_uppersmoke", "mp_player_int_smoke_enter", 1)},
{"1_23", new Animation("pro_mcs_7_concat-0", "cs_priest_dual-0", 48)},
{"1_24", new Animation("amb@code_human_in_car_mp_actions@dance@bodhi@ps@base", "idle_a_fp", 1)},
{"1_25", new Animation("amb@world_human_cop_idles@female@idle_b", "idle_d", 1)},
{"1_26", new Animation("move_action@p_m_one@unarmed@idle@variations", "idle_a", 1)},
{"1_27", new Animation("amb@world_human_cop_idles@female@idle_a", "idle_c", 1)},
{"1_28", new Animation("anim@heists@team_respawn@respawn_02", "heist_spawn_02_ped_d", 0)},
{"1_29", new Animation("mp_safehouseshower@male@", "male_shower_idle_d", 1)},
{"1_30", new Animation("amb@world_human_guard_patrol@male@idle_a", "idle_b", 1)},
{"1_31", new Animation("amb@prop_human_movie_studio_light@idle_a", "idle_a", 1)},
{"1_32", new Animation("gestures@m@standing@casual", "gesture_i_will", 48)},
{"1_33", new Animation("rcmpaparazzo1", "idle", 1)},
{"1_34", new Animation("rcmfanatic1out_of_breath", "p_zero_tired_01", 49)},
{"1_35", new Animation("anim@mp_player_intcelebrationfemale@shush", "shush", 0)},
{"1_36", new Animation("anim@heists@ornate_bank@hostages@hit", "player_melee_long_pistol_a", 0)},
{"1_37", new Animation("missfam2_washing_face", "michael_washing_face", 1)},
{"1_38", new Animation("random@arrests", "generic_radio_chatter", 49)},
{"1_39", new Animation("amb@world_human_clipboard@male@idle_b", "idle_d", 49)},
{"1_40", new Animation("amb@world_human_strip_watch_stand@male_a@base", "base", 1)},
{"1_41", new Animation("amb@world_human_strip_watch_stand@male_a@idle_a", "idle_c", 1)},
{"1_42", new Animation("amb@world_human_strip_watch_stand@male_b@base", "base", 1)},
{"1_43", new Animation("amb@world_human_strip_watch_stand@male_c@base", "base", 1)},
{"1_44", new Animation("amb@medic@standing@timeofdeath@base", "base", 1)},
{"1_45", new Animation("amb@world_human_bum_wash@male@high@idle_a", "idle_a", 0)},
{"1_46", new Animation("amb@world_human_leaning@female@coffee@base", "base", 1)},
{"1_47", new Animation("amb@code_human_police_crowd_control@idle_a", "idle_a", 1)},
{"1_48", new Animation("amb@lo_res_idles@", "world_human_bum_slumped_left_lo_res_base", 1)},
{"1_49", new Animation("amb@prop_human_bum_shopping_cart@male@idle_a", "idle_a", 1)},

{"2_1", new Animation("anim@amb@business@cfm@cfm_machine_no_work@", "transition_sleep_operator", 1)},
{"2_2", new Animation("switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_mic", 1)},
{"2_3", new Animation("mp_army_contact", "positive_a", 1)},
{"2_4", new Animation("random@robbery", "sit_down_idle_01", 1)},
{"2_5", new Animation("anim@amb@nightclub@peds@", "anim_heists_heist_safehouse_intro_phone_couch_female", 1)},
{"2_6", new Animation("anim@amb@business@cfid@cfid_desk_no_work_bgen_chair_no_work@", "transition_sleep_lazyworker", 1)},
{"2_7", new Animation("timetable@maid@couch@", "base", 1)},
{"2_8", new Animation("amb@code_human_in_car_mp_actions@v_sign@std@rds@base", "enter", 0)},
{"2_9", new Animation("missfam5_yoga", "c1_pose", 1)},
{"2_10", new Animation("missfam5_yoga", "a2_pose", 1)},
{"2_11", new Animation("amb@code_human_police_investigate@base", "base", 49)},
{"2_12", new Animation("amb@world_human_cop_idles@female@base", "base", 49)},
{"2_13", new Animation("mp_corona@single_team", "single_team_intro_boss", 1)},
{"2_14", new Animation("mp_corona@single_team", "single_team_loop_boss", 1)},
{"2_15", new Animation("anim@heists@ornate_bank@hostages@cashier_b@", "flinch_loop_underfire", 1)},
{"2_16", new Animation("anim@miss@low@fin@vagos@", "idle_ped06", 49)},
{"2_17", new Animation("random@peyote@fish", "wakeup_loop", 1)},
{"2_18", new Animation("amb@world_human_jog_standing@male@base", "base", 1)},
{"2_19", new Animation("amb@world_human_jog_standing@female@idle_a", "idle_a", 1)},
{"2_20", new Animation("amb@world_human_bum_slumped@male@laying_on_right_side@base", "base", 1)},
{"2_21", new Animation("amb@world_human_sunbathe@male@front@base", "base", 1)},
{"2_22", new Animation("missfbi1", "cpr_pumpchest_idle", 1)},
{"2_23", new Animation("amb@world_human_sunbathe@male@back@base", "base", 1)},
{"2_24", new Animation("amb@world_human_sunbathe@male@back@idle_a", "idle_a", 1)},
{"2_25", new Animation("missfam5_yoga", "a2_to_a3", 1)},
{"2_26", new Animation("missfbi4mcs_2", "loop_sec_b", 1)},
{"2_27", new Animation("amb@world_human_muscle_flex@arms_in_front@base", "base", 1)},
{"2_28", new Animation("anim@miss@low@fin@lamar@", "idle", 2)},
{"2_29", new Animation("amb@medic@standing@tendtodead@enter", "enter", 2)},
{"2_30", new Animation("amb@medic@standing@kneel@base", "base", 2)},
{"2_31", new Animation("amb@code_human_cower@male@react_cowering", "base_front", 1)},
{"2_32", new Animation("timetable@tracy@ig_5@idle_b", "idle_d", 1)},
{"2_33", new Animation("rcmme_amanda1", "stand_loop_cop", 49)},
{"2_34", new Animation("amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_b", 0)},
{"2_35", new Animation("amb@world_human_leaning@female@wall@back@holding_elbow@base", "base", 1)},
{"2_36", new Animation("amb@world_human_leaning@female@wall@back@holding_elbow@idle_a", "idle_a", 1)},
{"2_37", new Animation("amb@world_human_leaning@male@wall@back@hands_together@base", "base", 1)},
{"2_38", new Animation("amb@world_human_leaning@male@wall@back@legs_crossed@base", "base", 1)},
{"2_39", new Animation("amb@world_human_leaning@male@wall@back@foot_up@react_shock", "front", 1)},
{"2_40", new Animation("amb@world_human_leaning@male@wall@back@hands_together@idle_b", "idle_e", 1)},
{"2_41", new Animation("amb@world_human_leaning@male@wall@back@legs_crossed@idle_a", "idle_c", 1)},
{"2_42", new Animation("amb@world_human_leaning@male@wall@back@smoking@base", "base", 1)},
{"2_43", new Animation("amb@world_human_picnic@male@base", "base", 1)},
{"2_44", new Animation("amb@world_human_picnic@male@idle_a", "idle_a", 1)},
{"2_45", new Animation("rcm_barry3", "barry_3_sit_loop", 1)},
{"2_46", new Animation("amb@world_human_sunbathe@female@back@base", "base", 1)},
{"2_47", new Animation("amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_c", 49)},
{"2_48", new Animation("amb@world_human_car_park_attendant@male@base", "base", 1)},
{"2_49", new Animation("rcmcollect_paperleadinout@", "meditiate_idle", 1)},
{"2_50", new Animation("mp_player_int_upperarse_pick", "mp_player_int_arse_pick", 49)},
{"2_51", new Animation("rcmepsilonism8", "base_carrier", 1)},

{"3_1", new Animation("anim@mp_player_intincarthumbs_upbodhi@ds@", "enter_fp", 0)},
{"3_2", new Animation("amb@world_human_cheering@female_a", "base", 1)},
{"3_3", new Animation("missmic_4premiere", "movie_prem_01_f_a", 1)},
{"3_4", new Animation("anim@mp_player_intcelebrationfemale@freakout", "freakout", 0)},
{"3_5", new Animation("mini@dartsoutro", "darts_outro_03_guy2", 0)},
{"3_6", new Animation("mini@dartsoutro", "darts_outro_01_guy1", 0)},
{"3_7", new Animation("amb@code_human_police_crowd_control@idle_a", "idle_c", 0)},
{"3_8", new Animation("missmic_4premiere", "movie_prem_02_f_a", 49)},
{"3_9", new Animation("amb@world_human_cheering@male_e", "base", 49)},
{"3_10", new Animation("amb@world_human_cheering@male_a", "base", 1)},
{"3_11", new Animation("anim@mp_player_intcelebrationfemale@blow_kiss", "blow_kiss", 49)},
{"3_12", new Animation("mp_ped_interaction", "kisses_guy_b", 0)},
{"3_13", new Animation("anim@mp_player_intcelebrationmale@bro_love", "bro_love", 49)},
{"3_14", new Animation("anim@mp_player_intcelebrationfemale@thumbs_up", "thumbs_up", 0)},
{"3_15", new Animation("anim@mp_player_intupperthumbs_up", "idle_a_fp", 1)},
{"3_16", new Animation("anim@mp_point", "1st_person_high_blocked", 1)},

{"4_1", new Animation("anim@mp_player_intcelebrationfemale@nose_pick", "nose_pick", 1)},
{"4_2", new Animation("gestures@m@standing@casual", "gesture_nod_no_hard", 48)},
{"4_3", new Animation("taxi_hail", "forget_it", 0)},
{"4_4", new Animation("anim@mp_player_intcelebrationfemale@knuckle_crunch", "knuckle_crunch", 49)},
{"4_5", new Animation("mini@dartsoutro", "darts_outro_03_guy1", 0)},
{"4_6", new Animation("anim@mp_parachute_outro@female@lose", "lose_loop", 0)},
{"4_7", new Animation("anim@mp_player_intcelebrationfemale@you_loco", "you_loco", 0)},
{"4_8", new Animation("anim@mp_player_intcelebrationfemale@face_palm", "face_palm", 49)},
{"4_9", new Animation("amb@world_human_superhero@male@space_pistol@idle_a", "idle_b", 0)},
{"4_10", new Animation("anim@deathmatch_intros@2hcombat_mgmale", "intro_male_mg_c", 0)},
{"4_11", new Animation("anim@deathmatch_intros@melee@2h", "intro_male_melee_2h_b_gclub", 0)},
{"4_12", new Animation("anim@deathmatch_intros@unarmed", "intro_male_unarmed_b", 0)},
{"4_13", new Animation("oddjobs@towingangryidle_a", "idle_c", 1)},
{"4_14", new Animation("anim@mp_player_intupperfinger", "idle_a_fp", 49)},
{"4_15", new Animation("anim@mp_player_intcelebrationmale@face_palm", "face_palm", 49)},
{"4_16", new Animation("anim@mp_player_intcelebrationmale@cry_baby", "cry_baby", 0)},
{"4_17", new Animation("anim@mp_player_intcelebrationfemale@jazz_hands", "jazz_hands", 0)},
{"4_18", new Animation("anim@mp_player_intcelebrationfemale@thumb_on_ears", "thumb_on_ears", 0)},
{"4_19", new Animation("anim@mp_player_intcelebrationmale@thumb_on_ears", "thumb_on_ears", 0)},
{"4_20", new Animation("anim@mp_player_intcelebrationmale@cut_throat", "cut_throat", 1)},
{"4_21", new Animation("anim@mp_player_intcelebrationmale@you_loco", "you_loco", 1)},
{"4_22", new Animation("anim@mp_player_intcelebrationmale@freakout", "freakout", 1)},
{"4_23", new Animation("anim@mp_player_intcelebrationmale@stinker", "stinker", 1)},
{"4_24", new Animation("amb@code_human_police_investigate@idle_a", "idle_c", 0)},

{"5_1", new Animation("anim@mp_player_intcelebrationfemale@dj", "dj", 1)},
{"5_2", new Animation("misscarsteal4@toilet", "desperate_toilet_idle_a", 1)},
{"5_3", new Animation("special_ped@mountain_dancer@monologue_2@monologue_2a", "mnt_dnc_angel", 1)},
{"5_4", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_center", 1)},
{"5_5", new Animation("switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_stripper", 1)},
{"5_6", new Animation("anim@amb@nightclub@djs@black_madonna@", "dance_a_loop_blamadon", 1)},
{"5_7", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_a@", "high_center", 1)},
{"5_8", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_a@", "high_center_up", 1)},
{"5_9", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_a@", "med_center_up", 1)},
{"5_10", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_a@", "med_left", 1)},
{"5_11", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^1", 1)},
{"5_12", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^1", 1)},
{"5_13", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^3", 1)},
{"5_14", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^2", 1)},
{"5_15", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^4", 1)},
{"5_16", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_female^3", 1)},
{"5_17", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v2_female^3", 1)},
{"5_18", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v2_male^5", 1)},
{"5_19", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_15_v1_male^1", 1)},
{"5_20", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_15_v2_female^3", 1)},
{"5_21", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_17_v2_female^3", 1)},
{"5_22", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_female^1", 1)},
{"5_23", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_female^3", 1)},
{"5_24", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^1", 1)},
{"5_25", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^3", 1)},
{"5_26", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_11_v1_male^3", 1)},
{"5_27", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_15_v2_male^4", 1)},
{"5_28", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_17_v2_female^2", 1)},
{"5_29", new Animation("anim@amb@nightclub@lazlow@hi_dancefloor@", "crowddance_hi_11_handup_laz", 1)},
{"5_30", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_15_crazyrobot_laz", 1)},
{"5_31", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_17_smackthat_laz", 1)},
{"5_32", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_17_spiderman_laz", 1)},
{"5_33", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_mi_17_crotchgrab_laz", 1)},
{"5_34", new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_09_mi_hi_bellydancer_laz", 1)},
{"5_35", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "high_center", 1)},
{"5_36", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "high_center_down", 1)},
{"5_37", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "low_center_up", 1)},
{"5_38", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "high_center_up", 1)},
{"5_39", new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@", "high_center_down", 1)},
{"5_40", new Animation("misschinese2_crystalmazemcs1_cs", "dance_loop_tao", 1)},
{"5_41", new Animation("misschinese2_crystalmazemcs1_ig", "dance_loop_tao", 1)},
{"5_42", new Animation("move_clown@p_m_two_idles@", "fidget_short_dance", 1)},
{"5_43", new Animation("rcmnigel1bnmt_1b", "dance_loop_tyler", 1)},
{"5_44", new Animation("missfbi3_sniping", "dance_m_default", 1)},
{"5_45", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "med_center", 1)},
{"5_46", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_center", 1)},
{"5_47", new Animation("anim@amb@nightclub@peds@", "mini_strip_club_lap_dance_ld_girl_a_song_a_p1", 1)},
{"5_48", new Animation("mini@strip_club@lap_dance@ld_girl_a_song_a_p2", "ld_girl_a_song_a_p2_f", 1)},
{"5_49", new Animation("mp_am_stripper", "lap_dance_girl", 1)},
{"5_50", new Animation("mini@strip_club@private_dance@part2", "priv_dance_p2", 1)},
{"5_51", new Animation("mini@strip_club@private_dance@part3", "priv_dance_p3", 1)},
{"5_52", new Animation("oddjobs@assassinate@multi@yachttarget@lapdance", "yacht_ld_f", 1)},
{"5_53", new Animation("mp_safehouse", "lap_dance_girl", 1)},
{"5_54", new Animation("mini@strip_club@lap_dance_2g@ld_2g_p2", "ld_2g_p2_s1", 1)},
{"5_55", new Animation("amb@world_human_prostitute@cokehead@idle_a", "idle_a", 1)},
{"5_56", new Animation("amb@world_human_prostitute@cokehead@idle_a", "idle_b", 1)},
{"5_57", new Animation("amb@world_human_prostitute@cokehead@idle_a", "idle_c", 1)},
{"5_58", new Animation("mini@strip_club@lap_dance_2g@ld_2g_p1", "ld_2g_p1_s1", 1)},
{"5_59", new Animation("mini@strip_club@lap_dance_2g@ld_2g_p1", "ld_2g_p1_s2", 1)},

{"6_1", new Animation("anim@mp_player_intcelebrationfemale@air_guitar", "air_guitar", 0)},
{"6_2", new Animation("anim@mp_player_intcelebrationmale@peace", "peace", 0)},
{"6_3", new Animation("anim@mp_player_intcelebrationfemale@chicken_taunt", "chicken_taunt", 0)},
{"6_4", new Animation("creatures@rottweiler@melee@", "victim_takedown_from_front", 0)},
{"6_5", new Animation("amb@code_human_in_car_mp_actions@rock@bodhi@rps@base", "idle_a", 48)},
{"6_6", new Animation("amb@code_human_in_car_mp_actions@rock@bodhi@rps@base", "idle_a", 48)},
{"6_7", new Animation("random@gang_intimidation@", "001445_01_gangintimidation_1_female_wave_loop", 49)},
{"6_8", new Animation("anim@mp_player_intcelebrationmale@karate_chops", "karate_chops", 1)},
{"6_9", new Animation("anim@melee@machete@streamed_core@", "victim_front_takedown", 1)},
{"6_10", new Animation("anim@mp_player_intcelebrationfemale@photography", "photography", 0)},

{"7_1", new Animation("majestic_animations", "blinding_lights", 0)},
{"7_2", new Animation("majestic_animations", "boogie_down", 1)},
{"7_3", new Animation("majestic_animations", "cowboy_dance", 1)},
{"7_4", new Animation("majestic_animations", "crossbounce", 1)},
{"7_5", new Animation("majestic_animations", "disco_dance", 1)},
{"7_6", new Animation("majestic_animations", "dont_start_now", 1)},
{"7_7", new Animation("majestic_animations", "floss_dance", 1)},
{"7_8", new Animation("majestic_animations", "fresh", 1)},
{"7_9", new Animation("majestic_animations", "gangnam_style", 33)},
{"7_10", new Animation("majestic_animations", "i_heart_you", 0)},
{"7_11", new Animation("majestic_animations", "jabba_switchway", 1)},
{"7_12", new Animation("majestic_animations", "macarena", 1)},
{"7_13", new Animation("majestic_animations", "last_forever", 1)},
{"7_14", new Animation("majestic_animations", "ridethepony_v2", 1)},
{"7_15", new Animation("majestic_animations", "rollie", 1)},
{"7_16", new Animation("majestic_animations", "say_so", 1)},
{"7_17", new Animation("majestic_animations", "shuffle2", 1)},
{"7_18", new Animation("majestic_animations", "squat_kick", 1)},
{"7_19", new Animation("majestic_animations", "step_it_up", 0)},
{"7_20", new Animation("majestic_animations", "the_flow", 1)},
{"7_21", new Animation("majestic_animations_2", "renegade", 1)},
{"7_22", new Animation("majestic_animations_2", "stuck", 1)},
{"7_23", new Animation("majestic_animations_2", "pump_up", 1)},
{"7_24", new Animation("majestic_animations_2", "socks", 1)},
{"7_25", new Animation("majestic_animations_2", "my_world", 1)},
{"7_26", new Animation("majestic_animations_2", "wake_up", 1)},
{"7_27", new Animation("majestic_animations_2", "onda", 1)},
{"7_28", new Animation("majestic_animations_2", "gridy", 33)},
{"7_29", new Animation("majestic_animations_2", "hit_it", 1)},
{"7_30", new Animation("majestic_animations_2", "leave_door_open", 1)},
{"7_31", new Animation("majestic_animations_2", "chicken_wing", 1)},
{"7_32", new Animation("majestic_animations_2", "savage", 1)},
{"7_33", new Animation("majestic_animations_2", "electro_swing", 1)},
{"7_34", new Animation("majestic_animations_2", "sprinkler", 1)},
{"7_35", new Animation("majestic_animations_3", "smeeze", 1)},
{"7_36", new Animation("majestic_animations_3", "mufasa", 33)},
{"7_37", new Animation("majestic_animations_3", "hey_now", 1)},
{"7_38", new Animation("majestic_animations_3", "build_up", 1)},
{"7_39", new Animation("majestic_animations_3", "take_the_l", 1)},
{"7_40", new Animation("majestic_animations", "hip_hop", 1)},
{"7_41", new Animation("majestic_animations_3", "i_aint_afraid", 1)},
{"7_42", new Animation("majestic_animations_3", "get_gone", 1)},
{"7_43", new Animation("majestic_animations_3", "maximum_bounce", 1)},
{"7_44", new Animation("majestic_animations_3", "like_to_move", 33)},
{"7_45", new Animation("majestic_animations_3", "leilt_elomr", 1)},
{"7_46", new Animation("majestic_animations_3", "tidy", 1)},
{"7_47", new Animation("majestic_animations_3", "bhangra_boogie", 1)},
{"7_48", new Animation("majestic_animations_3", "out_west", 1)},
{"7_49", new Animation("majestic_animations_3", "toosie_slide", 1)},
{"7_50", new Animation("majestic_animations_3", "pull_up", 1)},
{"7_51", new Animation("majestic_animations_3", "the_crane_kick", 0)},
{"7_52", new Animation("majestic_animations_3", "billy_bounce", 1)},
{"7_53", new Animation("majestic_animations_3", "electro_shuffle", 1)},
{"7_54", new Animation("majestic_animations_4", "work_it_out", 1)},
{"7_55", new Animation("majestic_animations_2", "zany", 1)},
{"7_56", new Animation("majestic_animations_3", "smooth_moves", 1)},
{"7_57", new Animation("majestic_animations_4", "vivacious", 1)},
{"7_58", new Animation("majestic_animations_4", "hula", 1)},
{"7_59", new Animation("majestic_animations_4", "true_heart", 1)},
{"7_60", new Animation("majestic_animations_4", "reanimated", 1)},
{"7_61", new Animation("majestic_animations_4", "in_da_party", 33)},
{"7_62", new Animation("majestic_animations_4", "bim_bam_boom", 1)},
{"7_63", new Animation("majestic_animations_4", "wanna_see_me", 1)},
{"7_64", new Animation("majestic_animations_4", "dynamic_shuffle", 1)},
{"7_65", new Animation("majestic_animations_4", "never_gonna", 1)},
{"7_66", new Animation("majestic_animations_4", "fright_funk", 1)},
{"7_67", new Animation("majestic_animations_4", "jitterbug", 1)},
{"7_68", new Animation("majestic_animations_4", "infectious", 1)},
{"7_69", new Animation("majestic_animations_4", "where_is_matt", 1)},
{"7_70", new Animation("majestic_animations_4", "savor_the_w", 1)},
{"7_71", new Animation("majestic_animations_5", "dance_therapy", 1)},
{"7_72", new Animation("majestic_animations_5", "intensity", 1)},
{"7_73", new Animation("majestic_animations_5", "rushin_around", 1)},
{"7_74", new Animation("majestic_animations_5", "advanced_math", 1)},
{"7_75", new Animation("majestic_animations_5", "bold_stance", 1)},
{"7_76", new Animation("majestic_animations_5", "freemix", 1)},
{"7_77", new Animation("majestic_animations_5", "extraterrestrial", 1)},
{"7_78", new Animation("majestic_animations_5", "crabby", 1)},
{"7_79", new Animation("majestic_animations_5", "lavish", 1)},
{"7_80", new Animation("majestic_animations_5", "mime_time", 1)},
{"7_81", new Animation("majestic_animations_5", "tai_chi", 1)},
{"7_82", new Animation("majestic_animations_props", "hydraulics_player", 33)},
{"7_83", new Animation("majestic_animations_5", "daydream", 1)},
{"7_84", new Animation("majestic_animations_5", "work_it", 1)},
{"7_85", new Animation("majestic_animations_5", "slick", 1)},
{"7_86", new Animation("majestic_animations_5", "bombastic", 1)},
{"7_87", new Animation("majestic_animations_5", "its_a_vibe", 1)},
{"7_88", new Animation("majestic_animations_5", "wutang_is_forever", 1)},
{"7_89", new Animation("majestic_animations_5", "rootin_tootin", 0)},
{"7_90", new Animation("majestic_animations_5", "triumphant", 1)},
{"7_91", new Animation("majestic_animations_props", "alfredo_player", 33)},
{"7_92", new Animation("majestic_animations_props", "layers_player", 1)},
{"7_93", new Animation("majestic_animations_props", "epic_sax", 1)},
{"7_94", new Animation("majestic_animations_props", "llama_cowbell", 1)},
{"7_95", new Animation("majestic_animations_props", "majestic_flipped", 0)},
{"7_96", new Animation("majestic_animations_props", "llama_float_player", 33)},
{"7_97", new Animation("majestic_animations_props", "glowstick_dance", 1)},
{"7_98", new Animation("majestic_animations_props", "shake_dance", 1)},
{"7_99", new Animation("majestic_animations_props", "ukulele_dance", 1)},
{"7_100", new Animation("majestic_animations_props", "snare_solo_player", 1)},
{"7_101", new Animation("majestic_animations_props", "rock_out_player", 1)},
{"7_102", new Animation("majestic_animations_props", "rhyme_lock_player", 33)},
{"7_103", new Animation("majestic_animations_props", "unicycle_gadget_player", 33)},
{"7_104", new Animation("majestic_animations_props", "banner_flag_player", 1)},
{"7_105", new Animation("majestic_animations_props", "bouquet_hat_player", 0)},
{"7_106", new Animation("majestic_animations_props", "car_lifted_player", 33)},
{"7_107", new Animation("majestic_animations_props", "guitar_walk_player", 1)},
{"7_108", new Animation("majestic_animations_6", "best_mates", 1)},
{"7_109", new Animation("majestic_animations_6", "on_your_marks", 0)},
{"7_110", new Animation("majestic_animations_6", "laid_back_shuffle", 1)},
{"7_111", new Animation("majestic_animations_6", "pollo_dance", 1)},
{"7_112", new Animation("majestic_animations_6", "scenario", 1)},
{"7_113", new Animation("majestic_animations_6", "buckle_up", 1)},
{"7_114", new Animation("majestic_animations_6", "its_complicated", 1)},
{"7_115", new Animation("majestic_animations_6", "freedom_wheels", 1)},
{"7_116", new Animation("majestic_animations_6", "everybody_loves_me", 1)},
{"7_117", new Animation("majestic_animations_6", "pirouette", 1)},
{"7_118", new Animation("majestic_animations_6", "lazer_blast", 1)},
{"7_119", new Animation("majestic_animations_6", "poki", 1)},
{"7_120", new Animation("majestic_animations_6", "leapin", 1)},
{"7_121", new Animation("majestic_animations_6", "well_rounded", 1)},
{"7_122", new Animation("majestic_animations_6", "flux", 1)},
{"7_123", new Animation("majestic_animations_6", "whirlwind", 1)},
{"7_124", new Animation("majestic_animations_6", "jamboree", 1)},
{"7_125", new Animation("majestic_animations_6", "slap_happy", 1)},
{"7_126", new Animation("majestic_animations_6", "dream_feet", 1)},
{"7_127", new Animation("majestic_animations_6", "switchstep", 1)},
{"7_128", new Animation("majestic_animations_6", "glitter", 1)},
{"7_129", new Animation("majestic_animations_6", "sugar_rush", 1)},
{"7_130", new Animation("majestic_animations_6", "twist", 1)},
{"7_131", new Animation("majestic_animations_6", "howl", 0)},
{"7_132", new Animation("majestic_animations_6", "crazy_feet", 1)},
{"7_133", new Animation("majestic_animations_6", "hot_marat", 1)},
{"7_134", new Animation("majestic_animations_6", "show_stopper", 1)},
{"7_135", new Animation("majestic_animations_6", "boneless", 1)},
{"7_136", new Animation("majestic_animations_6", "pop_lock", 1)},
{"7_137", new Animation("majestic_animations_6", "steady", 1)},
{"7_138", new Animation("majestic_animations_6", "shimmer", 1)},
{"7_139", new Animation("majestic_animations_6", "springy", 1)},
{"7_140", new Animation("majestic_animations_6", "free_flow", 1)},
{"7_141", new Animation("majestic_animations_6", "conga", 1)},
{"7_142", new Animation("majestic_animations_6", "deep_end", 1)},
{"7_143", new Animation("majestic_animations_6", "pumpernickel", 1)},
{"7_144", new Animation("majestic_animations_6", "jubilation", 1)},
{"7_145", new Animation("majestic_animations_6", "jaywalking", 1)},
{"7_146", new Animation("majestic_animations_6", "peace_out", 0)},
{"7_147", new Animation("majestic_animations_6", "hype", 1)},
{"7_148", new Animation("majestic_animations_6", "orange_justice", 1)},
{"7_149", new Animation("majestic_animations_6", "swipe_it", 1)},
{"7_150", new Animation("majestic_animations_7", "jump_around", 33)},
{"7_151", new Animation("majestic_animations_7", "monster_mash", 1)},
{"7_152", new Animation("majestic_animations_7", "feel_the_flow", 1)},
{"7_153", new Animation("majestic_animations_7", "copines", 1)},
{"7_154", new Animation("majestic_animations_7", "jiggle", 1)},
{"7_155", new Animation("majestic_animations_7", "forget_me", 1)},
{"7_156", new Animation("majestic_animations_7", "chilled", 1)},
{"7_157", new Animation("majestic_animations_7", "distraction", 1)},
{"7_158", new Animation("majestic_animations_7", "ucan_cme", 0)},
{"7_159", new Animation("majestic_animations_props", "taco_time", 1)},
{"7_160", new Animation("majestic_animations_props_2", "snowglobe", 0)},
{"7_161", new Animation("majestic_animations_props_2", "mj_sleigh_player", 33)},
{"7_162", new Animation("majestic_animations_props_2", "sing_along_player", 1)},
{"7_163", new Animation("majestic_animations_props_2", "unwrapped_player", 0)},
{"7_164", new Animation("majestic_animations_7", "double_up", 1)},
{"7_165", new Animation("majestic_animations_7", "sway_1", 1)},
{"7_166", new Animation("majestic_animations_7", "its_dynamite", 1)},
{"7_167", new Animation("majestic_animations_props_2", "mayahi_player", 1)},
{"7_168", new Animation("majestic_animations_props_2", "jug", 1)},
{"7_169", new Animation("majestic_animations_props_2", "get_swifty_1", 1)},
{"7_170", new Animation("majestic_animations_props_2", "shanty", 33)},
{"7_171", new Animation("majestic_animations_props_2", "prancer_player", 33)},
{"7_172", new Animation("majestic_animations_props_2", "slalom_player", 33)},
{"7_173", new Animation("majestic_animations_7", "bounce_wit_it", 1)},
{"7_174", new Animation("majestic_animations_7", "dance_monkey", 1)},
{"7_175", new Animation("majestic_animations_8", "side_shuffle", 1)},
{"7_176", new Animation("majestic_animations_8", "flapper", 1)},
{"7_177", new Animation("majestic_animations_8", "vibin", 1)},
{"7_178", new Animation("majestic_animations_8", "the_robot", 1)},
{"7_179", new Animation("majestic_animations_8", "groove_jam", 1)},
{"7_180", new Animation("majestic_animations_8", "flamenco", 1)},
{"7_181", new Animation("majestic_animations_8", "rick_dance", 1)},
{"7_182", new Animation("majestic_animations_8", "crackdown", 1)},
{"7_183", new Animation("majestic_animations_8", "primo_moves", 1)},
{"7_184", new Animation("majestic_animations_8", "balletic", 1)},
{"7_185", new Animation("majestic_animations_8", "infinite_dab", 1)},
{"7_186", new Animation("majestic_animations_8", "hand_signals", 0)},
{"7_187", new Animation("majestic_animations_8", "fancy_feet", 1)},
{"7_188", new Animation("majestic_animations_8", "clean_groove", 1)},
{"7_189", new Animation("majestic_animations_8", "old_school", 1)},
{"7_190", new Animation("majestic_animations_8", "introducing", 1)},
{"7_191", new Animation("majestic_animations_8", "terrestrial", 1)},
{"7_192", new Animation("majestic_animations_8", "youre_awesome", 0)},
{"7_193", new Animation("majestic_animations_8", "cluck_strut", 1)},
{"7_194", new Animation("majestic_animations_8", "slitherin", 1)},
{"7_195", new Animation("majestic_animations_8", "its_go_time", 1)},
{"7_196", new Animation("majestic_animations_8", "get_funky", 1)},
{"7_197", new Animation("majestic_animations_8", "nana_nana", 1)},
{"7_198", new Animation("majestic_animations_8", "side_hustle", 1)},
{"7_199", new Animation("majestic_animations_8", "droop", 1)},
{"7_200", new Animation("majestic_animations_8", "mashed_potato", 1)},
{"7_201", new Animation("majestic_animations_8", "verve", 1)},
{"7_202", new Animation("majestic_animations_8", "gloss", 1)},
{"7_203", new Animation("majestic_animations_8", "my_idol", 1)},
{"7_204", new Animation("majestic_animations_9", "paws_claws", 1)},
{"7_205", new Animation("majestic_animations_9", "running_man", 1)},
{"7_206", new Animation("majestic_animations_9", "living_large", 1)},
{"7_207", new Animation("majestic_animations_9", "hootenanny", 1)},
{"7_208", new Animation("majestic_animations_9", "dirtbike_challenge", 1)},
{"7_209", new Animation("majestic_animations_9", "lunar_party", 1)},
{"7_210", new Animation("majestic_animations_9", "the_look", 1)},
{"7_211", new Animation("majestic_animations_9", "revel", 1)},
{"7_212", new Animation("majestic_animations_9", "im_diamond", 1)},
{"7_213", new Animation("majestic_animations_9", "hitchhiker", 1)},
{"7_214", new Animation("majestic_animations_9", "waterworks", 1)},
{"7_215", new Animation("majestic_animations_9", "pick_it_up", 1)},
{"7_216", new Animation("majestic_animations_9", "california_gurls", 1)},
{"7_217", new Animation("majestic_animations_9", "bboom_bboom", 1)},
{"7_218", new Animation("majestic_animations_10", "hang_loose_celebration", 1)},
{"7_219", new Animation("majestic_animations_10", "tootsee", 1)},
{"7_220", new Animation("majestic_animations_10", "the_dance_laroi", 1)},
{"7_221", new Animation("majestic_animations_10", "dance_off", 1)},
{"7_222", new Animation("majestic_animations_10", "fishy_flourish", 1)},
{"7_223", new Animation("majestic_animations_10", "freestylin", 1)},
{"7_224", new Animation("majestic_animations_10", "glyphic", 1)},
{"7_225", new Animation("majestic_animations_10", "fandalangle", 1)},
{"7_226", new Animation("majestic_animations_10", "marsh_walk", 1)},
{"7_227", new Animation("majestic_animations_10", "lazy_shuffle", 1)},
{"7_228", new Animation("majestic_animations_10", "backstroke", 1)},
{"7_229", new Animation("majestic_animations_10", "criss_cross", 1)},
{"7_230", new Animation("majestic_animations_10", "party_hips", 1)},
{"7_231", new Animation("majestic_animations_10", "llama_conga", 1)},
{"7_232", new Animation("majestic_animations_10", "jumping_jacks", 1)},
{"7_233", new Animation("majestic_animations_10", "shout", 1)},
{"7_234", new Animation("majestic_animations_10", "yay", 1)},
{"7_235", new Animation("majestic_animations_9", "forever", 1)},
{"7_236", new Animation("majestic_animations_10", "the_magic_bomb", 1)},
{"7_237", new Animation("majestic_animations_10", "roll_n_rock", 1)},
{"7_238", new Animation("majestic_animations_11", "warm_up", 1)},
{"7_239", new Animation("majestic_animations_11", "gungslinger_smokeshow", 1)},
{"7_240", new Animation("majestic_animations_11", "sweet_shot", 1)},
{"7_241", new Animation("majestic_animations_11", "vibrant_vibin", 1)},
{"7_242", new Animation("majestic_animations_11", "koi_dance", 1)},
{"7_243", new Animation("majestic_animations_11", "quick_style", 1)},
{"7_244", new Animation("majestic_animations_11", "made_you_look", 1)},
{"7_245", new Animation("majestic_animations_11", "ask_me", 1)},
{"7_246", new Animation("majestic_animations_props_3", "atomic_synth_player", 1)},
{"7_247", new Animation("majestic_animations_props_2", "sled_player", 33)},
{"7_248", new Animation("majestic_animations_props_3", "ring_it_on", 1)},
{"7_249", new Animation("majestic_animations_props_3", "boombox_player", 1)},
{"7_250", new Animation("majestic_animations_props_3", "boots_n_cats_player", 1)},
{"7_251", new Animation("majestic_animations_props_3", "mic_stand_player", 1)},
{"7_252", new Animation("majestic_animations_props_3", "declare", 49)},
{"7_253", new Animation("majestic_animations_props_3", "rocket_rodeo_player", 33)},
{"7_254", new Animation("majestic_animations_props_3", "drum_major_player", 1)},
{"7_255", new Animation("majestic_animations_props_3", "pump_it_up", 1)},
{"7_256", new Animation("majestic_animations_props_3", "cheer_up", 1)},
{"7_257", new Animation("majestic_animations_props_3", "empress_fan_dance", 1)},
{"7_258", new Animation("majestic_animations_dances_1", "manera", 1)},
{"7_259", new Animation("majestic_animations_11", "air_shredder", 1)},
{"7_260", new Animation("majestic_animations_11", "crazy_boy", 1)},
{"7_261", new Animation("majestic_animations_11", "fishin", 33)},
{"7_262", new Animation("majestic_animations_11", "ninja_style", 1)},
{"7_263", new Animation("majestic_animations_11", "the_worm", 1)},
{"7_264", new Animation("majestic_animations_11", "wiggle", 1)},
{"7_265", new Animation("majestic_animations_11", "star_power", 1)},
{"7_266", new Animation("majestic_animations_12", "rambunctious", 1)},
{"7_267", new Animation("majestic_animations_12", "rawr", 0)},
{"7_268", new Animation("majestic_animations_12", "fast_feet", 1)},
{"7_269", new Animation("majestic_animations_12", "capoeira", 1)},
{"7_270", new Animation("majestic_animations_12", "bobbin", 1)},
{"7_271", new Animation("majestic_animations_12", "overdrive", 1)},
{"7_272", new Animation("majestic_animations_12", "fanciful", 33)},
{"7_273", new Animation("majestic_animations_12", "bunny_hop", 33)},
{"7_274", new Animation("majestic_animations_12", "no_sweat", 1)},
{"7_275", new Animation("majestic_animations_12", "windmill_floss", 1)},
{"7_276", new Animation("majestic_animations_12", "swole_cat", 1)},
{"7_277", new Animation("majestic_animations_12", "head_banger", 1)},
{"7_278", new Animation("majestic_animations_12", "get_loose", 1)},
{"7_279", new Animation("majestic_animations_12", "bully", 1)},
{"7_280", new Animation("majestic_animations_12", "bring_it_around", 1)},
{"7_281", new Animation("majestic_animations_12", "square_up", 1)},
{"7_282", new Animation("majestic_animations_12", "without_you", 1)},
{"7_283", new Animation("majestic_animations_12", "run_it_down", 1)},
{"7_284", new Animation("majestic_animations_12", "goated", 1)},
{"7_285", new Animation("majestic_animations_12", "celebrate_me", 1)},
{"7_286", new Animation("majestic_animations_12", "pay_it_off", 1)},
{"7_287", new Animation("majestic_animations_12", "fast_flex", 1)},
{"7_288", new Animation("majestic_animations_12", "get_out_of_your_mind", 1)},
{"7_289", new Animation("majestic_animations_dances_1", "lit_dance", 1)},
{"7_290", new Animation("majestic_animations_props_7", "take_the_elf", 1)},
{"7_291", new Animation("majestic_animations_props_7", "snowman_player", 0)},
{"7_292", new Animation("majestic_animations_props_6", "choice_knit_player", 0)},
{"7_293", new Animation("majestic_animations_props_6", "shaolin_sip", 1)},
{"7_294", new Animation("majestic_animations_props_6", "treat_player", 33)},
{"7_295", new Animation("majestic_animations_props_6", "sparkler", 49)},
{"7_296", new Animation("majestic_animations_props_6", "telekinetic_player", 0)},
{"7_297", new Animation("majestic_animations_props_6", "tangerine_player", 49)},
{"7_298", new Animation("majestic_animations_props_7", "heart_attach_player", 0)},
{"7_299", new Animation("majestic_animations_props_7", "omg_love_player", 1)},
{"7_300", new Animation("majestic_animations_13", "planetary_vibe", 1)},
{"7_301", new Animation("majestic_animations_13", "pump_me_up", 1)},
{"7_302", new Animation("majestic_animations_13", "headbanger_2", 1)},
{"7_303", new Animation("majestic_animations_13", "culture_festival", 1)},
{"7_304", new Animation("majestic_animations_13", "bust_a_move_1", 1)},
{"7_305", new Animation("majestic_animations_13", "boys_a_liar", 1)},
{"7_306", new Animation("majestic_animations_13", "bizcochito", 1)},
{"7_307", new Animation("majestic_animations_13", "night_out", 1)},
{"7_308", new Animation("majestic_animations_13", "start_it_up", 1)},
{"7_309", new Animation("majestic_animations_13", "wind_up", 1)},
{"7_310", new Animation("majestic_animations_13", "starlit", 1)},
{"7_311", new Animation("majestic_animations_dances_1", "dom_yes", 1)},
{"7_312", new Animation("majestic_animations_dances_1", "wanna_dance", 1)},
{"7_313", new Animation("majestic_animations_props_4", "called_shot", 1)},
{"7_314", new Animation("majestic_animations_props_4", "witch_way_player", 33)},
{"7_315", new Animation("majestic_animations_props_4", "cardistry_player", 1)},
{"7_316", new Animation("majestic_animations_props_4", "target_training", 1)},
{"7_317", new Animation("majestic_animations_props_4", "crispy", 1)},
{"7_318", new Animation("majestic_animations_props_4", "sprout_of_tune_player", 1)},
{"7_319", new Animation("majestic_animations_props_4", "click_click_flash", 1)},
{"7_320", new Animation("majestic_animations_props_4", "pony_up", 1)},
{"7_321", new Animation("majestic_animations_props_8", "kiss_kiss", 0)},
{"7_322", new Animation("majestic_animations_props_8", "heart_sign", 49)},
{"7_323", new Animation("majestic_animations_14", "ambitious", 1)},
{"7_324", new Animation("majestic_animations_14", "bad_guy", 1)},
{"7_325", new Animation("majestic_animations_14", "boney_bounce", 1)},
{"7_326", new Animation("majestic_animations_14", "bood_up_groove", 1)},
{"7_327", new Animation("majestic_animations_14", "carefree", 1)},
{"7_328", new Animation("majestic_animations_14", "classy", 1)},
{"7_329", new Animation("majestic_animations_14", "dancin_domino", 1)},
{"7_330", new Animation("majestic_animations_14", "evil_plan", 1)},
{"7_331", new Animation("majestic_animations_14", "go_with_the_flow", 1)},
{"7_332", new Animation("majestic_animations_14", "hooray", 1)},
{"7_333", new Animation("majestic_animations_14", "jubi_slide", 33)},
{"7_334", new Animation("majestic_animations_14", "make_some_waves", 1)},
{"7_335", new Animation("majestic_animations_14", "no_cure", 1)},
{"7_336", new Animation("majestic_animations_14", "popular_vibe", 1)},
{"7_337", new Animation("majestic_animations_14", "rain_check", 1)},
{"7_338", new Animation("majestic_animations_14", "real_slim_shady", 1)},
{"7_339", new Animation("majestic_animations_14", "rebellious", 1)},
{"7_340", new Animation("majestic_animations_14", "show_ya", 1)},
{"7_341", new Animation("majestic_animations_14", "social_climber", 1)},
{"7_342", new Animation("majestic_animations_14", "swag_shuffle", 1)},
{"7_343", new Animation("majestic_animations_14", "the_squabble", 1)},
{"7_344", new Animation("majestic_animations_14", "to_the_beat", 1)},
{"7_345", new Animation("majestic_animations_14", "you_a_winner", 1)},
{"7_346", new Animation("majestic_animations_14", "you_should_see_me_in_a_crown", 1)},
{"7_347", new Animation("majestic_animations_dances_1", "just_wanna_rock", 1)},
{"7_348", new Animation("majestic_animations_dances_1", "9mm_go_bang", 1)},
{"7_349", new Animation("majestic_animations_props_10", "expressionism_player", 1)},
{"7_350", new Animation("majestic_animations_props_10", "squeezie_does_it_player", 33)},
{"7_351", new Animation("majestic_animations_props_9", "supercar_player", 33)},
{"7_352", new Animation("majestic_animations_props_10", "squirrelly_player", 0)},
{"7_353", new Animation("majestic_animations_props_10", "zoidberg_scuttle_player", 33)},
{"7_354", new Animation("majestic_animations_props_11", "ggwp_player", 0)},
{"7_355", new Animation("majestic_animations_props_10", "baller", 1)},
{"7_356", new Animation("majestic_animations_custom_4", "alpha_warrior_loop", 1)},
{"7_357", new Animation("majestic_animations_props_12", "live_from_haddonfield_player", 49)},
{"7_358", new Animation("majestic_animations_props_12", "troops_player", 0)},
{"7_359", new Animation("majestic_animations_props_11", "miracle_trickshot", 0)},
{"7_360", new Animation("majestic_animations_props_11", "reaper_showtime", 1)},
{"7_361", new Animation("majestic_animations_props_12", "pickin", 1)},
{"7_362", new Animation("majestic_animations_props_12", "billy_tricycle_player", 33)},
{"7_363", new Animation("majestic_animations_props_12", "prisoner_mugshot_player", 0)},
{"7_364", new Animation("majestic_animations_props_12", "prisoner_mugshot_player", 0)},
{"7_365", new Animation("majestic_animations_props_12", "samurai_strike", 0)},
{"7_366", new Animation("majestic_animations_props_12", "kioto_fan", 33)},
{"7_367", new Animation("majestic_animations_17", "ronald_greeting", 1)},
{"7_368", new Animation("majestic_animations_props_11", "pick_me_up_player", 33)},
{"7_369", new Animation("majestic_animations_props_10", "ice_moves_player", 0)},
{"7_370", new Animation("majestic_animations_props_8", "snowflakey_player", 0)},
{"7_371", new Animation("majestic_animations_props_8", "tea_time", 33)},
{"7_372", new Animation("majestic_animations_props_13", "chugga_train_player", 33)},
{"7_373", new Animation("majestic_animations_props_13", "money_blastin_player", 0)},
{"7_374", new Animation("majestic_animations_props_13", "snow_airboard", 33)},
{"7_375", new Animation("majestic_animations_props_13", "toys_flip_player", 49)},
{"7_376", new Animation("majestic_animations_props_13", "deer_cadabra_player", 0)},
{"7_377", new Animation("majestic_animations_props_13", "cloud_swing_player", 1)},
{"7_378", new Animation("majestic_animations_props_12", "boing_player", 33)},
{"7_379", new Animation("majestic_animations_17", "bye_bye_bye", 1)},
{"7_380", new Animation("majestic_animations_17", "caffeinated", 1)},
{"7_381", new Animation("majestic_animations_17", "desirable", 1)},
{"7_382", new Animation("majestic_animations_17", "what_you_want", 1)},
{"7_383", new Animation("majestic_animations_15", "entranced", 1)},
{"7_384", new Animation("majestic_animations_15", "company_jig", 1)},
{"7_385", new Animation("majestic_animations_15", "heartbreak_shuffle_1", 1)},
{"7_386", new Animation("majestic_animations_15", "point_and_strut", 1)},
{"7_387", new Animation("majestic_animations_16", "breakneck", 1)},
{"7_388", new Animation("majestic_animations_16", "no_tears", 1)},
{"7_389", new Animation("majestic_animations_15", "lofi_headbang", 1)},
{"7_390", new Animation("majestic_animations_15", "brite_moves", 1)},
{"7_391", new Animation("majestic_animations_17", "waddle_away", 33)},
{"7_392", new Animation("majestic_animations_props_14", "gift_drop_player", 1)},
{"7_393", new Animation("majestic_animations_props_14", "santa_xoxo_player", 1)},
{"7_394", new Animation("majestic_animations_props_14", "xoxo_female_player", 0)},
{"7_395", new Animation("majestic_animations_props_14", "shrek_donkey_player", 33)},
{"7_396", new Animation("majestic_animations_dances_2", "alaska_puffer", 1)},
{"7_397", new Animation("majestic_animations_props_15", "nya_arigato_player", 1)},
{"7_398", new Animation("majestic_animations_dances_3", "shine", 1)},
{"7_399", new Animation("majestic_animations_dances_3", "exum_shuffle", 1)},
{"7_400", new Animation("majestic_animations_dances_3", "lemon_melon_coockie", 1)},
{"7_401", new Animation("majestic_animations_dances_3", "sweet_bumble_bee", 1)},
{"7_402", new Animation("majestic_animations_15", "surfin_bird", 1)},
{"7_403", new Animation("majestic_animations_17", "deep_explorer", 1)},
{"7_404", new Animation("majestic_animations_17", "lucid_dreams", 1)},
{"7_405", new Animation("majestic_animations_17", "smitted", 1)},
{"7_406", new Animation("majestic_animations_17", "commited_player_one", 1)},
{"7_407", new Animation("majestic_animations_props_8", "ribbon_dance_player", 1)},
{"7_408", new Animation("majestic_animations_props_15", "amazing_cube_player", 49)},
{"7_409", new Animation("majestic_animations_props_15", "asmr_keys", 1)},
{"7_410", new Animation("majestic_animations_props_15", "cupid_arrow_player", 1)},
{"7_411", new Animation("majestic_animations_props_15", "miku_live_player", 1)},
{"7_412", new Animation("majestic_animations_props_15", "accolades_player", 1)},
{"7_413", new Animation("majestic_animations_props_16", "dill_with_it_player", 0)},
{"7_414", new Animation("majestic_animations_props_16", "popcorn_player", 49)},
{"7_415", new Animation("majestic_animations_props_16", "surrender_player", 48)},
{"7_416", new Animation("majestic_animations_props_10", "jump_rope_jig_player", 1)},
{"7_417", new Animation("majestic_animations_props_16", "tick_tock_player", 0)},
{"7_418", new Animation("majestic_animations_props_16", "wcash_player", 1)},
{"7_419", new Animation("majestic_animations_props_16", "money_rain_player", 1)},
{"7_420", new Animation("majestic_animations_props_10", "ballsy_player", 1)},
{"7_421", new Animation("majestic_animations_props_16", "light_weight", 1)},
{"7_422", new Animation("majestic_animations_props_16", "target_player", 49)},
{"7_423", new Animation("majestic_animations_props_8", "tornado_spin_player", 1)},
{"7_424", new Animation("majestic_animations_props_10", "kick_ups_player", 1)},
{"7_425", new Animation("majestic_animations_props_16", "cleansing_flame", 1)},
{"7_426", new Animation("majestic_animations_18", "close_to_you_1_player", 1)},
{"7_427", new Animation("majestic_animations_18", "attraction", 1)},
{"7_428", new Animation("majestic_animations_18", "california_love", 1)},
{"7_429", new Animation("majestic_animations_18", "dimensional", 1)},
{"7_430", new Animation("majestic_animations_18", "keep_me_satisfied", 1)},
{"7_431", new Animation("majestic_animations_18", "click", 0)},
{"7_432", new Animation("majestic_animations_dances_4", "lovely_signs", 0)},
{"7_433", new Animation("majestic_animations_dances_4", "pokemon_dance", 1)},
{"7_434", new Animation("majestic_animations_dances_5", "doodle", 1)},
{"7_435", new Animation("majestic_animations_dances_4", "crank_it_up", 1)},
{"7_436", new Animation("majestic_animations_dances_4", "anxiety", 1)},
{"7_437", new Animation("majestic_animations_dances_4", "apt", 1)},
{"7_438", new Animation("majestic_animations_dances_5", "wait_challenge", 1)},
{"7_439", new Animation("majestic_animations_dances_5", "el_paso_beam", 1)},
{"7_440", new Animation("majestic_animations_dances_5", "lezginka_custom", 1)},




        };
        public static void AnimationStop(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.AnimationUse != null && AnimList.ContainsKey(sessionData.AnimationUse))
                {
                    var category = sessionData.AnimationUse;
                    Trigger.StopAnimation(player);
                    
                    if (AnimList[category].Flag == 39) 
                        NAPI.Entity.SetEntityPosition(player, player.Position + new Vector3(0, 0, 0.2));
                    
                    sessionData.HandsUp = false;
                    Trigger.ClientEvent(player, "client.animation.isPlayer", 0);
                }
                sessionData.AnimationUse = null;
            }
            catch (Exception e)
            {
                Log.Write($"AnimationStop Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.animation.play")]
        public static void AnimationPlay(ExtPlayer player, string category, bool vehCheck = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.AntiAnimDown || sessionData.Following != null || sessionData.SitPos != -1 || sessionData.InAirsoftLobby >= 0) return;
                if (vehCheck && player.IsInVehicle) return;
                if (sessionData.AnimationUse != null && sessionData.AnimationUse == category) return;
                if (!AnimList.ContainsKey(category))
                {
                    AnimationStop(player);
                    return;
                }
                var animData = AnimList[category];
                sessionData.AnimationUse = category;
                Trigger.PlayAnimation(player, animData.Dictionary, animData.Name, animData.Flag);
                Trigger.ClientEvent(player, "client.animation.isPlayer", 1);
                
                BattlePass.Repository.UpdateReward(player, 70);
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage10, 1, isUpdateHud: true);
                qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage10, true);
                
                if (animData.Dictionary == "random@arrests@busted" && animData.Name == "idle_c") 
                    sessionData.HandsUp = true;
                else
                    sessionData.HandsUp = false;
            }
            catch (Exception e)
            {
                Log.Write($"animationSelected Exception: {e.ToString()}");
            }
        }
        public static void playerInteractionTarget(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!target.IsCharacterData() || player.Position.DistanceTo(target.Position) > 3) return;

                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null || targetSessionData.AnimationUse == null || !AnimList.ContainsKey(targetSessionData.AnimationUse))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerDontAnim), 3000);
                    return;
                }
                AnimationPlay(player, targetSessionData.AnimationUse);
                BattlePass.Repository.UpdateReward(player, 56);
            }
            catch (Exception e)
            {
                Log.Write($"playerInteractionTarget Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.animation.favorite")]
        public void AnimationDavorite(ExtPlayer player, string json)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                characterData.ConfigData.AnimFavorites = json;
            }
            catch (Exception e)
            {
                Log.Write($"AnimationDavorite Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.animation.bind")]
        public void AnimationBind(ExtPlayer player, string json)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                characterData.ConfigData.AnimBind = json;
            }
            catch (Exception e)
            {
                Log.Write($"AnimationDavorite Exception: {e.ToString()}");
            }
        }

        [Command("starta")]
        public static void CMD_starta(ExtPlayer player)
        {
            if (Main.ServerNumber != 0) return;
            Trigger.ClientEvent(player, "test.taskPlayAnim");
        }
    }
}
