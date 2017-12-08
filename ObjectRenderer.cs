/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SM64DSe
{
    public class ObjectRenderer
    {
        public static ObjectRenderer FromLevelObject(LevelObject obj)
        {
            ObjectRenderer ret = null;

            switch (obj.ID)
            {
                // 0 -- PLAYER -- TODO
                case 1: ret = new NormalBMDRenderer("data/special_obj/ewb_ice/ewb_ice_a.bmd", 0.008f); break;
                case 2: ret = new NormalBMDRenderer("data/special_obj/ewb_ice/ewb_ice_b.bmd", 0.008f); break;
                case 3: ret = new NormalBMDRenderer("data/special_obj/ewb_ice/ewb_ice_c.bmd", 0.008f); break;
                case 4: ret = new NormalBMDRenderer("data/special_obj/ewm_ice_brock/ewm_ice_brock.bmd", 0.008f); break;
                case 5: ret = new NormalBMDRenderer("data/special_obj/emm_log/emm_log.bmd", 0.008f); break;
                case 6: ret = new NormalBMDRenderer("data/special_obj/emm_yuka/emm_yuka.bmd", 0.008f); break;
                case 7: ret = new NormalBMDRenderer("data/normal_obj/obj_updnlift/obj_updnlift.bmd", 0.008f); break;
                case 8: ret = new NormalBMDRenderer("data/special_obj/hs_updown_lift/hs_updown_lift.bmd", 0.008f); break;
                case 9: ret = new NormalBMDRenderer("data/normal_obj/obj_pathlift/obj_pathlift.bmd", 0.008f); break;
                case 10: ret = new ChainedWanWanRenderer(); break;
                // 11 -- CAMERA_TAG -- non-graphical
                case 12: ret = new NormalBMDRenderer("data/normal_obj/obj_seesaw/obj_seesaw.bmd", 0.008f); break;
                case 13: ret = new NormalBMDRenderer("data/enemy/iron_ball/iron_ball.bmd", 0.008f); break;
                case 14: ret = new NormalBMDRenderer("data/special_obj/cv_goro_rock/cv_goro_rock.bmd", 0.008f); break;
                case 15: ret = new NormalBMDRenderer("data/enemy/kuribo/kuribo_model.bmd", 0.008f); break;
                case 16: ret = new NormalBMDRenderer("data/enemy/kuribo/kuribo_model.bmd", 0.002f); break;
                case 17: ret = new NormalBMDRenderer("data/enemy/kuribo/kuribo_model.bmd", 0.016f); break;
                case 18: ret = new NormalBMDRenderer("data/enemy/kuriking/kuriking_model.bmd", 0.008f); break;
                case 19: ret = new NormalBMDRenderer("data/enemy/bombhei/bombhei.bmd", 0.008f); break;
                case 20: ret = new NormalBMDRenderer("data/enemy/bombhei/red_bombhei.bmd", 0.008f); break;
                case 21: ret = new NormalBMDRenderer("data/enemy/nokonoko/nokonoko" + ((obj.Parameters[0] & 1) != 0 ? "_red" : "") + ".bmd", 0.008f); break;
                case 22: ret = new NormalBMDRenderer("data/enemy/nokonoko/shell_" + ((obj.Parameters[0] & 1) != 0 ? "red" : "green") + ".bmd", 0.008f); break;
                case 23: ret = new NormalBMDRenderer("data/normal_obj/obj_block/broken_block_l.bmd", 0.008f); break;
                case 24: ret = new NormalBMDRenderer("data/normal_obj/obj_block/broken_block_l.bmd", 0.012f); break;
                case 25: ret = new NormalBMDRenderer("data/normal_obj/obj_block/broken_block_l.bmd", 0.008f); break;
                case 26: ret = new NormalBMDRenderer("data/normal_obj/obj_power_flower/p_flower_open.bmd", 0.008f); break;
                case 27: ret = new NormalBMDRenderer("data/normal_obj/obj_hatena_switch/hatena_switch.bmd", 0.008f); break;
                case 28: ret = new NormalBMDRenderer("data/normal_obj/obj_block/broken_block_s.bmd", 0.008f); break;
                case 29: ret = new NormalBMDRenderer("data/normal_obj/obj_cannon_shutter/cannon_shutter.bmd", 0.008f); break;
                case 30: ret = new NormalBMDRenderer("data/normal_obj/obj_hatena_box/hatena_box.bmd", 0.008f); break;
                case 31: ret = new NormalBMDRenderer("data/normal_obj/obj_hatena_box/obj_hatena_y_box.bmd", 0.008f); break;
                case 32: ret = new NormalBMDRenderer("data/normal_obj/obj_hatena_box/obj_hatena_y_box.bmd", 0.008f); break;
                case 33: ret = new NormalBMDRenderer("data/normal_obj/obj_hatena_box/obj_cap_box_m.bmd", 0.008f); break;
                case 34: ret = new NormalBMDRenderer("data/normal_obj/obj_hatena_box/obj_cap_box_w.bmd", 0.008f); break;
                case 35: ret = new NormalBMDRenderer("data/normal_obj/obj_hatena_box/obj_cap_box_l.bmd", 0.008f); break;
                case 36: ret = new NormalBMDRenderer("data/normal_obj/obj_pile/pile.bmd", 0.008f); break;
                case 37: ret = new NormalBMDRenderer("data/normal_obj/coin/coin_poly32.bmd", 0.008f); break;
                case 38: ret = new NormalBMDRenderer("data/normal_obj/coin/coin_red_poly32.bmd", 0.008f); break;
                case 39: ret = new NormalBMDRenderer("data/normal_obj/coin/coin_blue_poly32.bmd", 0.008f); break;
                case 40: ret = new NormalBMDRenderer("data/enemy/koopa/koopa_model.bmd", 0.008f); break;
                case 41: ret = new TreeRenderer((obj.Parameters[0] >> 4) & 0x7); break;
                case 42: ret = new PaintingRenderer(obj.Parameters[0], obj.Parameters[1]); break;
                case 43: ret = new NormalBMDRenderer("data/normal_obj/obj_box_switch/obj_box_switch.bmd", 0.008f); break;
                case 44: ret = new NormalBMDRenderer("data/normal_obj/obj_star_switch/obj_star_switch.bmd", 0.008f); break;
                case 45: ret = new NormalBMDRenderer("data/special_obj/b_ana_shutter/b_ana_shutter.bmd", 0.008f); break;
                case 46: ret = new NormalBMDRenderer("data/special_obj/cv_shutter/cv_shutter.bmd", 0.008f); break;
                case 47: ret = new NormalBMDRenderer("data/special_obj/cv_news_lift/cv_news_lift.bmd", 0.008f); break;
                case 48: ret = new LooseWanWanRenderer(); break;
                case 49: ret = new NormalBMDRenderer("data/normal_obj/oneup_kinoko/oneup_kinoko.bmd", 0.008f); break;
                case 50: ret = new NormalBMDRenderer("data/normal_obj/obj_cannon/houdai.bmd", 0.008f); break;
                case 51: ret = new NormalBMDRenderer("data/special_obj/b_wan_shutter/b_wan_shutter.bmd", 0.008f); break;
                case 52: ret = new NormalBMDRenderer("data/enemy/water_bomb/water_bomb.bmd", 0.008f); break;
                //case 53: ret = new NormalBMDRenderer("data/normal_obj/birds/birds.bmd", 0.008f); break;
                    // 54  FISH
                //case 55: ret = new NormalBMDRenderer("data/normal_obj/butterfly/butterfly.bmd", 0.008f); break;
                case 56: ret = new NormalBMDRenderer("data/enemy/bombking/bomb_king.bmd", 0.008f); break;
                case 57: ret = new NormalBMDRenderer("data/enemy/snowman/snowman_model.bmd", 0.008f); break;
                case 58: ret = new NormalBMDRenderer("data/enemy/piano/piano.bmd", 0.008f); break;
                case 59: ret = new NormalBMDRenderer("data/enemy/pakkun/pakkun_model.bmd", 0.008f); break;
                    // 60 STAR CAMERA
                case 61: ret = new StarRenderer(obj); break;
                case 62: ret = new NormalBMDRenderer("data/normal_obj/star/obj_star_silver.bmd", 0.008f); break;
                case 63: ret = new StarRenderer(obj); break;
                case 64: ret = new NormalBMDRenderer("data/enemy/battan/battan.bmd", 0.008f); break;
                case 65: ret = new NormalBMDRenderer("data/enemy/battan_king/battan_king.bmd", 0.008f); break;
                case 66: ret = new NormalBMDRenderer("data/enemy/dosune/dosune.bmd", 0.008f); break;
                case 67: ret = new NormalBMDRenderer("data/enemy/teresa/teresa.bmd", 0.008f); break;
                case 68: ret = new NormalBMDRenderer("data/enemy/boss_teresa/boss_teresa.bmd", 0.008f); break;
                    // 69 ICON TERESA
                //case 70: ret = new NormalBMDRenderer("data/special_obj/th_kaidan/th_kaidan.bmd", 0.008f); break;
                case 71: ret = new NormalBMDRenderer("data/special_obj/th_hondana/th_hondana.bmd", 0.008f); break;
                case 72: ret = new NormalBMDRenderer("data/special_obj/th_mery_go/th_mery_go.bmd", 0.008f); break;
                case 73: ret = new NormalBMDRenderer("data/special_obj/th_trap/th_trap.bmd", 0.008f); break;
                // 74 -- PL_CLOSET -- non-graphical
                case 75: ret = new NormalBMDRenderer("data/normal_obj/obj_kanban/obj_kanban.bmd", 0.008f); break;
                case 76: ret = new NormalBMDRenderer("data/normal_obj/obj_tatefuda/obj_tatefuda.bmd", 0.008f); break;
                case 77: ret = new NormalBMDRenderer("data/normal_obj/obj_ice_board/obj_ice_board.bmd", 0.008f); break;
                case 78: ret = new NormalBMDRenderer("data/normal_obj/obj_wakame/obj_wakame.bmd", 0.008f); break;
                case 79: ret = new NormalBMDRenderer("data/normal_obj/obj_heart/obj_heart.bmd", 0.008f); break;
                case 80: ret = new NormalBMDRenderer("data/enemy/kinopio/kinopio.bmd", 0.008f); break;
                case 81: ret = new NormalBMDRenderer("data/enemy/peach/peach_high.bmd", 0.008f); break;
                case 82: ret = new NormalBMDRenderer("data/special_obj/kb2_stage/kb2_stage.bmd", 0.008f); break;
                case 83: ret = new Koopa3bgRenderer(obj.Parameters[0] & 0xFF); break;
                case 84: ret = new NormalBMDRenderer("data/enemy/nokonoko/shell_green.bmd", 0.008f); break;
                case 85: ret = new NormalBMDRenderer("data/enemy/hojiro/hojiro.bmd", 0.008f); break;
                case 86: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj01/ct_mecha_obj01.bmd", 0.008f); break;
                case 87: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj02/ct_mecha_obj02.bmd", 0.008f); break;
                case 88: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj03/ct_mecha_obj03.bmd", 0.008f); break;
                case 89: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj04l/ct_mecha_obj04l.bmd", 0.008f); break;
                case 90: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj04s/ct_mecha_obj04s.bmd", 0.008f); break;
                case 91: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj05/ct_mecha_obj05.bmd", 0.008f); break;
                case 92: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj06/ct_mecha_obj06.bmd", 0.008f); break;
                case 93: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj07/ct_mecha_obj07.bmd", 0.008f); break;
                case 94: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj08a/ct_mecha_obj08a.bmd", 0.008f); break;
                case 95: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj08b/ct_mecha_obj08b.bmd", 0.008f); break;
                case 96: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj09/ct_mecha_obj09.bmd", 0.008f); break;
                case 97: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj10/ct_mecha_obj10.bmd", 0.008f); break;
                case 98: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj11/ct_mecha_obj11.bmd", 0.008f); break;
                case 99: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj12l/ct_mecha_obj12l.bmd", 0.008f); break;
                case 100: ret = new NormalBMDRenderer("data/special_obj/ct_mecha_obj12s/ct_mecha_obj12s.bmd", 0.008f); break;
                case 101: ret = new NormalBMDRenderer("data/special_obj/dp_brock/dp_brock.bmd", 0.008f); break;
                case 102: ret = new NormalBMDRenderer("data/special_obj/dp_lift/dp_lift.bmd", 0.008f); break;
                case 103: ret = new NormalBMDRenderer("data/special_obj/dl_pyramid/dl_pyramid.bmd", 0.008f); break;
                // 104 -- DL_PYRAMID_DUMMY -- non-graphical
                case 105: ret = new NormalBMDRenderer("data/special_obj/wl_pole_lift/wl_pole_lift.bmd", 0.008f); break;
                case 106: ret = new NormalBMDRenderer("data/special_obj/wl_submarine/wl_submarine.bmd", 0.008f); break;
                case 107: ret = new NormalBMDRenderer("data/special_obj/wl_kupa_shutter/wl_kupa_shutter.bmd", 0.008f); break;
                case 108: ret = new NormalBMDRenderer("data/special_obj/rc_dorifu/rc_dorifu0.bmd", 0.008f); break;
                case 109: ret = new NormalBMDRenderer("data/special_obj/rc_rift01/rc_rift01.bmd", 0.008f); break;
                case 110: ret = new NormalBMDRenderer("data/special_obj/rc_hane/rc_hane.bmd", 0.008f); break;
                case 111: ret = new NormalBMDRenderer("data/special_obj/rc_tikuwa/rc_tikuwa.bmd", 0.008f); break;
                case 112: ret = new NormalBMDRenderer("data/special_obj/rc_buranko/rc_buranko.bmd", 0.008f); break;
                case 113: ret = new NormalBMDRenderer("data/special_obj/rc_shiso/rc_shiso.bmd", 0.008f); break;
                case 114: ret = new NormalBMDRenderer("data/special_obj/rc_kaiten/rc_kaiten.bmd", 0.008f); break;
                case 115: ret = new NormalBMDRenderer("data/special_obj/rc_guruguru/rc_guruguru.bmd", 0.008f); break;
                case 116: ret = new NormalBMDRenderer("data/special_obj/sl_ice_brock/sl_ice_brock.bmd", 0.008f); break;
                case 117: ret = new NormalBMDRenderer("data/special_obj/hm_maruta/hm_maruta.bmd", 0.008f); break;
                case 118: ret = new NormalBMDRenderer("data/special_obj/tt_obj_futa/tt_obj_futa.bmd", 0.008f); break;
                case 119: ret = new NormalBMDRenderer("data/special_obj/tt_obj_water/tt_obj_water.bmd", 0.008f); break;
                case 120: ret = new NormalBMDRenderer("data/special_obj/td_obj_futa/td_obj_futa.bmd", 0.008f); break;
                case 121: ret = new NormalBMDRenderer("data/special_obj/td_obj_water/td_obj_water.bmd", 0.008f); break;
                case 122: ret = new NormalBMDRenderer("data/special_obj/wc_obj07/wc_obj07.bmd", 0.008f); break;
                case 123: ret = new NormalBMDRenderer("data/special_obj/wc_obj01/wc_obj01.bmd", 0.008f); break;
                case 124: ret = new NormalBMDRenderer("data/special_obj/wc_obj02/wc_obj02.bmd", 0.008f); break;
                case 125: ret = new NormalBMDRenderer("data/special_obj/wc_obj03/wc_obj03.bmd", 0.008f); break;
                case 126: ret = new NormalBMDRenderer("data/special_obj/wc_obj04/wc_obj04.bmd", 0.008f); break;
                case 127: ret = new NormalBMDRenderer("data/special_obj/wc_obj05/wc_obj05.bmd", 0.008f); break;
                case 128: ret = new NormalBMDRenderer("data/special_obj/wc_obj06/wc_obj06.bmd", 0.008f); break;
                case 129: ret = new NormalBMDRenderer("data/special_obj/wc_mizu/wc_mizu.bmd", 0.008f); break;
                case 130: ret = new NormalBMDRenderer("data/special_obj/fl_log/fl_log.bmd", 0.008f); break;
                case 131: ret = new NormalBMDRenderer("data/special_obj/fl_ring/fl_ring.bmd", 0.008f); break;
                case 132: ret = new NormalBMDRenderer("data/special_obj/fl_gura/fl_gura.bmd", 0.008f); break;
                case 133: ret = new NormalBMDRenderer("data/special_obj/fl_london/fl_london.bmd", 0.008f); break;
                case 134: ret = new NormalBMDRenderer("data/special_obj/fl_block/fl_block.bmd", 0.008f); break;
                case 135: ret = new NormalBMDRenderer("data/special_obj/fl_uki_yuka/fl_uki_yuka.bmd", 0.008f); break;
                case 136: ret = new NormalBMDRenderer("data/special_obj/fl_shiso/fl_shiso.bmd", 0.008f); break;
                case 137: ret = new NormalBMDRenderer("data/special_obj/fl_shiso/fl_shiso.bmd", 0.008f); break;
                case 138: ret = new NormalBMDRenderer("data/special_obj/fl_koma_d/fl_koma_d.bmd", 0.008f); break;
                case 139: ret = new NormalBMDRenderer("data/special_obj/fl_koma_u/fl_koma_u.bmd", 0.008f); break;
                case 140: ret = new NormalBMDRenderer("data/special_obj/fl_uki_ki/fl_uki_ki.bmd", 0.008f); break;
                case 141: ret = new NormalBMDRenderer("data/special_obj/fl_kuzure/fl_kuzure.bmd", 0.008f); break;
                case 142: ret = new NormalBMDRenderer("data/special_obj/fm_battan/fm_battan.bmd", 0.008f); break;
                    // 143-144
                case 145: ret = new NormalBMDRenderer("data/enemy/manta/manta.bmd", 0.008f); break;
                case 146: ret = new NormalBMDRenderer("data/enemy/spider/spider.bmd", 0.008f); break;
                case 147: ret = new NormalBMDRenderer("data/enemy/togezo/togezo.bmd", 0.008f); break;
                case 148: ret = new NormalBMDRenderer("data/enemy/jugem/jugem.bmd", 0.008f); break;
                case 149: ret = new NormalBMDRenderer("data/enemy/gamaguchi/gamaguchi.bmd", 0.008f); break;
                case 150: ret = new NormalBMDRenderer("data/enemy/eyekun/eyekun.bmd", 0.008f); break;
                case 151: ret = new NormalBMDRenderer("data/enemy/eyekun/eyekun.bmd", 0.016f); break;
                case 152: ret = new NormalBMDRenderer("data/enemy/batta_block/batta_block.bmd", 0.008f); break;
                case 153: ret = new DoubleRenderer("data/enemy/birikyu/birikyu.bmd", "data/enemy/birikyu/birikyu_elec.bmd", 0.008f); break;
                case 154: ret = new NormalBMDRenderer("data/special_obj/hm_basket/hm_basket.bmd", 0.008f); break;
                case 155: ret = new NormalBMDRenderer("data/enemy/monkey/monkey.bmd", 0.008f); break;
                    // 156 UKIKI (STAR)
                case 157: ret = new NormalBMDRenderer("data/enemy/penguin/penguin_child.bmd", 0.002f); break;
                case 158: ret = new NormalBMDRenderer("data/enemy/penguin/penguin.bmd", 0.008f); break;
                case 159: ret = new NormalBMDRenderer("data/enemy/penguin/penguin.bmd", 0.008f); break;
                case 160: ret = new NormalBMDRenderer("data/enemy/penguin/penguin.bmd", 0.008f); break;
                case 161: ret = new NormalBMDRenderer("data/enemy/keronpa/keronpa.bmd", 0.008f); break;
                case 162: ret = new BigSnowmanRenderer(); break;
                case 163: ret = new NormalBMDRenderer("data/enemy/big_snowman/big_snowman_head.bmd", 0.008f); break;
                case 164: ret = new NormalBMDRenderer("data/enemy/big_snowman/big_snowman_body.bmd", 0.008f); break;
                    // 165 SNOWMAN BREATH
                case 166: ret = new NormalBMDRenderer("data/enemy/pukupuku/pukupuku.bmd", 0.008f); break;
                case 167: ret = new NormalBMDRenderer("data/special_obj/c2_hari_short/c2_hari_short.bmd", 0.008f); break;
                case 168: ret = new NormalBMDRenderer("data/special_obj/c2_hari_long/c2_hari_long.bmd", 0.008f); break;
                case 169: ret = new NormalBMDRenderer("data/special_obj/c2_huriko/c2_huriko.bmd", 0.008f); break;
                case 170: ret = new NormalBMDRenderer("data/enemy/menbo/menbo.bmd", 0.008f); break;
                //case 171: ret = new NormalBMDRenderer("data/special_obj/casket/casket.bmd", 0.008f); break; BIG BOO'S HAUNT COFFIN, off-centre
                case 172: ret = new NormalBMDRenderer("data/enemy/hyuhyu/hyuhyu.bmd", 0.008f); break;
                case 173: ret = new NormalBMDRenderer("data/special_obj/b_si_so/b_si_so.bmd", 0.008f); break;
                case 174: ret = new NormalBMDRenderer("data/special_obj/km1_shiso/km1_shiso.bmd", 0.008f); break;
                case 175: ret = new NormalBMDRenderer("data/special_obj/km1_dorifu/km1_dorifu0.bmd", 0.008f); break;
                case 176: ret = new NormalBMDRenderer("data/special_obj/km1_ukishima/km1_ukishima.bmd", 0.008f); break;
                case 177: ret = new KurumajikuRenderer("km1"); break;
                case 178: ret = new NormalBMDRenderer("data/special_obj/km1_deru/km1_deru.bmd", 0.008f); break;
                //case 179: ret = new NormalBMDRenderer("data/special_obj/ki_fune/ki_fune_down_a.bmd", 0.008f); break;
                case 180: ret = new NormalBMDRenderer("data/special_obj/ki_fune/ki_fune_up.bmd", 0.008f); break;
                case 181: ret = new DoubleRenderer("data/special_obj/ki_hasira/ki_hasira_dai.bmd", 
                    "data/special_obj/ki_hasira/ki_hasira.bmd", Vector3.Zero, new Vector3(0f, 12.5f, 0f), 0.008f); break;
                case 182: ret = new NormalBMDRenderer("data/special_obj/ki_hasira/ki_hasira_dai.bmd", 0.008f); break;
                case 183: ret = new NormalBMDRenderer("data/special_obj/ki_ita/ki_ita.bmd", 0.008f); break;
                case 184: ret = new NormalBMDRenderer("data/special_obj/ki_iwa/ki_iwa.bmd", 0.008f); break;
                case 185: ret = new NormalBMDRenderer("data/special_obj/ks_mizu/ks_mizu.bmd", 0.008f); break;
                case 186: ret = new NormalBMDRenderer("data/normal_obj/obj_dokan/obj_dokan.bmd", 0.008f); break;
                case 187: ret = new NormalBMDRenderer("data/normal_obj/obj_yajirusi_l/yajirusi_l.bmd", 0.008f); break;
                case 188: ret = new NormalBMDRenderer("data/normal_obj/obj_yajirusi_r/yajirusi_r.bmd", 0.008f); break;
                case 189: ret = new NormalBMDRenderer("data/enemy/propeller_heyho/propeller_heyho.bmd", 0.008f); break;
                case 190: ret = new DoubleRenderer("data/enemy/killer/killer_body.bmd", "data/enemy/killer/killer_face.bmd", 0.008f); break;
                case 191: ret = new NormalBMDRenderer("data/special_obj/kb1_ball/kb1_ball.bmd", 0.008f); break;
                case 192: ret = new NormalBMDRenderer("data/special_obj/hs_moon/hs_moon.bmd", 0.008f); break;
                case 193: ret = new NormalBMDRenderer("data/special_obj/hs_star/hs_star.bmd", 0.008f); break;
                case 194: ret = new NormalBMDRenderer("data/special_obj/hs_y_star/hs_y_star.bmd", 0.008f); break;
                case 195: ret = new NormalBMDRenderer("data/special_obj/hs_b_star/hs_b_star.bmd", 0.008f); break;
                case 196: ret = new NormalBMDRenderer("data/special_obj/bk_billbord/bk_billbord.bmd", 0.008f); break;
                case 197: ret = new NormalBMDRenderer("data/special_obj/bk_killer_dai/bk_killer_dai.bmd", 0.008f); break;
                case 198: ret = new NormalBMDRenderer("data/special_obj/bk_botaosi/bk_botaosi.bmd", 0.008f); break;
                case 199: ret = new NormalBMDRenderer("data/special_obj/bk_down_b/bk_down_b.bmd", 0.008f); break;
                case 200: ret = new NormalBMDRenderer("data/special_obj/bk_futa/bk_futa.bmd", 0.008f); break;
                case 201: ret = new NormalBMDRenderer("data/special_obj/bk_kabe01/bk_kabe01.bmd", 0.008f); break;
                case 202: ret = new NormalBMDRenderer("data/special_obj/bk_kabe00/bk_kabe00.bmd", 0.008f); break;
                case 203: ret = new NormalBMDRenderer("data/special_obj/bk_tower/bk_tower.bmd", 0.008f); break;
                case 204: ret = new NormalBMDRenderer("data/special_obj/bk_ukisima/bk_ukisima.bmd", 0.008f); break;
                case 205: ret = new NormalBMDRenderer("data/special_obj/bk_rotebar/bk_rotebar.bmd", 0.008f); break;
                case 206: ret = new NormalBMDRenderer("data/special_obj/bk_lift01/bk_lift01.bmd", 0.008f); break;
                case 207: ret = new NormalBMDRenderer("data/special_obj/bk_dossunbar_s/bk_dossunbar_s.bmd", 0.008f); break;
                case 208: ret = new NormalBMDRenderer("data/special_obj/bk_dossunbar_l/bk_dossunbar_l.bmd", 0.008f); break;
                case 209: ret = new NormalBMDRenderer("data/special_obj/bk_transbar/bk_transbar.bmd", 0.008f); break;
                case 210: ret = new NormalBMDRenderer("data/special_obj/th_down_b/th_down_b.bmd", 0.008f); break;
                case 211: ret = new NormalBMDRenderer("data/special_obj/km2_kuzure/km2_kuzure.bmd", 0.008f); break;
                case 212: ret = new NormalBMDRenderer("data/special_obj/km2_agaru/km2_agaru.bmd", 0.008f); break;
                case 213: ret = new NormalBMDRenderer("data/special_obj/km2_gura/km2_gura.bmd", 0.008f); break;
                case 214: ret = new NormalBMDRenderer("data/special_obj/km2_ami_bou/km2_ami_bou.bmd", 0.008f); break;
                case 215: ret = new NormalBMDRenderer("data/special_obj/km2_yokoshiso/km2_yokoshiso.bmd", 0.008f); break;
                case 216: ret = new NormalBMDRenderer("data/special_obj/km2_susumu/km2_susumu.bmd", 0.008f); break;
                case 217: ret = new NormalBMDRenderer("data/special_obj/km2_ukishima/km2_ukishima.bmd", 0.008f); break;
                case 218: ret = new NormalBMDRenderer("data/special_obj/km2_rift02/km2_rift02.bmd", 0.008f); break;
                case 219: ret = new NormalBMDRenderer("data/special_obj/km2_rift01/km2_rift01.bmd", 0.008f); break;
                case 220: ret = new NormalBMDRenderer("data/special_obj/km2_nobiru/km2_nobiru.bmd", 0.008f); break;
                case 221: ret = new NormalBMDRenderer("data/special_obj/km3_shiso/km3_shiso.bmd", 0.008f); break;
                case 222: ret = new NormalBMDRenderer("data/special_obj/km3_yokoshiso/km3_yokoshiso.bmd", 0.008f); break;
                case 223: ret = new KurumajikuRenderer("km3"); break;
                case 224: ret = new NormalBMDRenderer("data/special_obj/km3_dan/km3_dan0.bmd", 0.008f); break;
                case 225: ret = new NormalBMDRenderer("data/special_obj/km3_deru01/km3_deru01.bmd", 0.008f); break;
                case 226: ret = new NormalBMDRenderer("data/special_obj/km3_deru02/km3_deru02.bmd", 0.008f); break;
                case 227: ret = new NormalBMDRenderer("data/special_obj/km3_kaitendai/km3_kaitendai.bmd", 0.008f); break;
                case 228: ret = new NormalBMDRenderer("data/special_obj/c0_switch/c0_switch.bmd", 0.008f); break;
                case 229: ret = new NormalBMDRenderer("data/special_obj/sm_lift/sm_lift.bmd", 0.008f); break;
                // case 230: ret = new NormalBMDRenderer("data/special_obj/fl_log/fl_log.bmd", 0.008f); break; // WRONG
                case 231: ret = new NormalBMDRenderer("data/special_obj/th_lift/th_lift.bmd", 0.008f); break;
                case 232: ret = new NormalBMDRenderer("data/special_obj/cv_ud_lift/cv_ud_lift.bmd", 0.008f); break;
                case 233: ret = new NormalBMDRenderer("data/special_obj/rc_rift02/rc_rift02.bmd", 0.008f); break;
                case 234: ret = new NormalBMDRenderer("data/enemy/bakubaku/bakubaku.bmd", 0.008f); break;
                case 235: ret = new NormalBMDRenderer("data/special_obj/km3_rift/km3_rift.bmd", 0.008f); break;
                case 236: ret = new NormalBMDRenderer("data/enemy/koopa_bomb/koopa_bomb.bmd", 0.008f); break;
                case 237: ret = new NormalBMDRenderer("data/enemy/mip/mip.bmd", 0.008f); break;
                    // 238 RABBIT KEY
                case 239: ret = new NormalBMDRenderer("data/enemy/owl/owl.bmd", 0.008f); break;
                case 240: ret = new NormalBMDRenderer("data/enemy/donketu/donketu.bmd", 0.008f); break;
                case 241: ret = new NormalBMDRenderer("data/enemy/donketu/boss_donketu.bmd", 0.008f); break;
                case 242: ret = new ToxboxRenderer(); break;
                case 243: ret = new PoleRenderer(Color.FromArgb(0, 0, 255), Color.FromArgb(0, 0, 64), obj.Parameters[0]); break;
                case 244: ret = new NormalBMDRenderer("data/enemy/c_jugem/c_jugem.bmd", 0.008f); break;
                case 245: ret = new NormalBMDRenderer("data/normal_obj/obj_pushblock/obj_pushblock.bmd", 0.008f); break;
                case 246: ret = new NormalBMDRenderer("data/special_obj/fl_amilift/fl_amilift.bmd", 0.008f); break;
                case 247: ret = new NormalBMDRenderer("data/enemy/yurei_mucho/yurei_mucho.bmd", 0.008f); break;
                case 248: ret = new NormalBMDRenderer("data/enemy/choropu/choropu.bmd", 0.008f); break;
                case 249: ret = new NormalBMDRenderer("data/enemy/choropu/rock.bmd", 0.008f); break;
                case 250: ret = new NormalBMDRenderer("data/enemy/basabasa/basabasa.bmd", 0.008f); break;
                case 251: ret = new NormalBMDRenderer("data/enemy/popoi/popoi.bmd", 0.008f); break;
                case 252: ret = new NormalBMDRenderer("data/enemy/jango/jango.bmd", 0.008f); break;
                case 253: ret = new PokeyRenderer(); break;
                    // 254 MARIO CAP
                case 255: ret = new FlPuzzleRenderer(obj.Parameters[0] & 0xFF); break;
                // 256 -- FL_COIN -- non-graphical
                case 257: ret = new NormalBMDRenderer("data/enemy/dossy/dossy.bmd", 0.008f); break;
                    // 258 DOSSY CAP
                case 259: ret = new NormalBMDRenderer("data/enemy/huwahuwa/huwahuwa_model.bmd", 0.008f); break;
                case 260: ret = new NormalBMDRenderer("data/special_obj/ki_slide_box/ki_slide_box.bmd", 0.008f); break;
                case 261: ret = new NormalBMDRenderer("data/enemy/moray/moray.bmd", 0.008f); break;
                case 262: ret = new NormalBMDRenderer("data/normal_obj/obj_kumo/obj_kumo.bmd", 0.008f); break;
                case 263: ret = new NormalBMDRenderer("data/normal_obj/obj_shell/obj_shell.bmd", 0.008f); break;
                    // 264-272
                case 273: ret = new C1TrapRenderer(); break;
                case 274: ret = new NormalBMDRenderer("data/special_obj/c1_hikari/c1_hikari.bmd", 0.008f); break;
                case 275: ret = new NormalBMDRenderer("data/special_obj/c1_peach/c1_peach.bmd", 0.008f); break;
                case 276: ret = new NormalBMDRenderer("data/special_obj/rc_carpet/rc_carpet.bmd", 0.008f); break;
                // case 277: ret = new NormalBMDRenderer("data/normal_obj/koopa_key/koopa_key.bmd", 0.016f); break;
                    // 278
                case 279: ret = new NormalBMDRenderer("data/enemy/iwante/iwante_dummy.bmd", 0.008f); break; // TODO: Show as hands?
                case 280: ret = new WigglerRenderer(); break;
                case 281: ret = new NormalBMDRenderer("data/enemy/nokonoko/nokonoko.bmd", 0.01f); break;
                case 282: ret = new NormalBMDRenderer("data/normal_obj/obj_race_flag/obj_race_flag.bmd", 0.008f); break;
                case 283: ret = new NormalBMDRenderer("data/special_obj/t_basket/t_basket.bmd", 0.008f); break;
                case 284: ret = new NormalBMDRenderer("data/normal_obj/obj_block/broken_block_ll.bmd", 0.008f); break;
                case 285: ret = new NormalBMDRenderer("data/normal_obj/obj_block/ice_block_ll.bmd", 0.008f); break;
                    // 286-289
                case 290: ret = new NormalBMDRenderer("data/enemy/donketu/ice_donketu.bmd", 0.008f); break;
                case 291: ret = new NormalBMDRenderer("data/enemy/king_ice_donketu/king_ice_donketu_model.bmd", 0.008f); break;
                case 292: ret = new NormalBMDRenderer("data/normal_obj/t_box/t_box.bmd", 0.008f); break;
                case 293: ret = new NormalBMDRenderer("data/special_obj/mc_water/mc_water.bmd", 0.008f); break;
                case 294: ret = new NormalBMDRenderer("data/enemy/chair/chair.bmd", 0.008f); break;
                case 295: ret = new NormalBMDRenderer("data/special_obj/mc_metalnet/mc_metalnet.bmd", 0.008f); break;
                case 296: ret = new NormalBMDRenderer("data/special_obj/mc_dodai/mc_dodai.bmd", 0.008f); break;
                case 297: ret = new NormalBMDRenderer("data/special_obj/mc_hazad/mc_hazad.bmd", 0.008f); break;
                case 298: ret = new NormalBMDRenderer("data/special_obj/mc_flag/mc_flag.bmd", 0.008f); break;
                case 299: ret = new NormalBMDRenderer("data/enemy/donkaku/donkaku.bmd", 0.008f); break;
                case 300: ret = new NormalBMDRenderer("data/enemy/donguru/donguru.bmd", 0.008f); break;
                case 301: ret = new NormalBMDRenderer("data/enemy/horuhei/horuhei.bmd", 0.008f); break;
                case 302: ret = new NormalBMDRenderer("data/normal_obj/scale_up_kinoko/scale_up_kinoko.bmd", 0.008f); break;
                case 303: ret = new NormalBMDRenderer("data/special_obj/c0_water/c0_water.bmd", 0.008f); break;
                // 304 -- SECRET_COIN -- non-graphical
                case 305: ret = new NormalBMDRenderer("data/normal_obj/b_coin_switch/b_coin_switch.bmd", 0.008f); break;
                    // 306-310
                case 311: ret = new NormalBMDRenderer("data/normal_obj/w_ring/w_ring.bmd", 0.016f); break;
                case 312: ret = new NormalBMDRenderer("data/enemy/pakkun/pakkun_model.bmd", 0.016f); break;
                case 313: ret = new NormalBMDRenderer("data/enemy/pakkun/pakkun_model.bmd", 0.004f); break;
                case 314: ret = new NormalBMDRenderer("data/enemy/pakkun/pakkun_model.bmd", 0.008f); break;
                    // 315-317
                case 318: ret = new NormalBMDRenderer("data/normal_obj/water_tatumaki/water_tatumaki.bmd", 0.008f); break;
                    // 319
                case 320: ret = new NormalBMDRenderer("data/enemy/sand_tornado/sand_tornado.bmd", 0.008f); break;
                    // 321-325

                default: ret = new ColorCubeRenderer(Color.FromArgb(0, 0, 255), Color.FromArgb(0,0,64), obj.SupportsRotation()); break;
            }

            ret.m_ObjUniqueID = obj.m_UniqueID;

            return ret;
        }

        public virtual void Release() { }

        public virtual bool GottaRender(RenderMode mode) { return false; }
        public virtual void Render(RenderMode mode) { }
        public virtual void UpdateRenderer() { }

        public virtual string GetFilename() { return m_Filename; }
        public virtual Vector3 GetScale() { return m_Scale; }

        public uint m_ObjUniqueID;

        public string m_Filename;
        public Vector3 m_Scale;
    }


    class ColorCubeRenderer : ObjectRenderer
    {
        public ColorCubeRenderer(Color border, Color fill, bool showaxes)
        {
            m_BorderColor = border;
            m_FillColor = fill;
            m_ShowAxes = showaxes;
        }

        public override bool GottaRender(RenderMode mode)
        {
            if (mode == RenderMode.Translucent) return false;
            else return true;
        }

        public override void Render(RenderMode mode)
        {
            const float s = 0.04f;

            if (mode != RenderMode.Picking)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Color4(m_FillColor);
                GL.Disable(EnableCap.Lighting);
            }

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-s, -s, -s);
            GL.Vertex3(-s, s, -s);
            GL.Vertex3(s, -s, -s);
            GL.Vertex3(s, s, -s);
            GL.Vertex3(s, -s, s);
            GL.Vertex3(s, s, s);
            GL.Vertex3(-s, -s, s);
            GL.Vertex3(-s, s, s);
            GL.Vertex3(-s, -s, -s);
            GL.Vertex3(-s, s, -s);
            GL.End();

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-s, s, -s);
            GL.Vertex3(-s, s, s);
            GL.Vertex3(s, s, -s);
            GL.Vertex3(s, s, s);
            GL.End();

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-s, -s, -s);
            GL.Vertex3(s, -s, -s);
            GL.Vertex3(-s, -s, s);
            GL.Vertex3(s, -s, s);
            GL.End();

            if (mode != RenderMode.Picking)
            {
                GL.LineWidth(2.0f);
                GL.Color4(m_BorderColor);

                GL.Begin(PrimitiveType.LineStrip);
                GL.Vertex3(s, s, s);
                GL.Vertex3(-s, s, s);
                GL.Vertex3(-s, s, -s);
                GL.Vertex3(s, s, -s);
                GL.Vertex3(s, s, s);
                GL.Vertex3(s, -s, s);
                GL.Vertex3(-s, -s, s);
                GL.Vertex3(-s, -s, -s);
                GL.Vertex3(s, -s, -s);
                GL.Vertex3(s, -s, s);
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(-s, s, s);
                GL.Vertex3(-s, -s, s);
                GL.Vertex3(-s, s, -s);
                GL.Vertex3(-s, -s, -s);
                GL.Vertex3(s, s, -s);
                GL.Vertex3(s, -s, -s);
                GL.End();

                if (m_ShowAxes)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    GL.Vertex3(0.0f, 0.0f, 0.0f);
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    GL.Vertex3(s * 2.0f, 0.0f, 0.0f);
                    GL.Color3(0.0f, 1.0f, 0.0f);
                    GL.Vertex3(0.0f, 0.0f, 0.0f);
                    GL.Color3(0.0f, 1.0f, 0.0f);
                    GL.Vertex3(0.0f, s * 2.0f, 0.0f);
                    GL.Color3(0.0f, 0.0f, 1.0f);
                    GL.Vertex3(0.0f, 0.0f, 0.0f);
                    GL.Color3(0.0f, 0.0f, 1.0f);
                    GL.Vertex3(0.0f, 0.0f, s * 2.0f);
                    GL.End();
                }
            }
        }


        private Color m_BorderColor, m_FillColor;
        private bool m_ShowAxes;
    }


    class PoleRenderer : ObjectRenderer
    {
        public PoleRenderer(Color border, Color fill, ushort param)
        {
            m_BorderColor = border;
            m_FillColor = fill;
            m_height = (byte)param * 0.01f;

        }

        public override bool GottaRender(RenderMode mode)
        {
            if (mode == RenderMode.Opaque) return false;
            else return true;
        }

        public override void Render(RenderMode mode)
        {
            const float s = 0.04f;

            if (mode != RenderMode.Picking)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Color4(Color.FromArgb(100, m_FillColor));
                GL.Disable(EnableCap.Lighting);
            }

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-s, -s, -s);
            GL.Vertex3(-s, m_height, -s);
            GL.Vertex3(s, -s, -s);
            GL.Vertex3(s, m_height, -s);
            GL.Vertex3(s, -s, s);
            GL.Vertex3(s, m_height, s);
            GL.Vertex3(-s, -s, s);
            GL.Vertex3(-s, m_height, s);
            GL.Vertex3(-s, -s, -s);
            GL.Vertex3(-s, m_height, -s);
            GL.End();

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-s, m_height, -s);
            GL.Vertex3(-s, m_height, s);
            GL.Vertex3(s, m_height, -s);
            GL.Vertex3(s, m_height, s);
            GL.End();

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-s, -s, -s);
            GL.Vertex3(s, -s, -s);
            GL.Vertex3(-s, -s, s);
            GL.Vertex3(s, -s, s);
            GL.End();

            if (mode != RenderMode.Picking)
            {
                GL.LineWidth(2.0f);
                GL.Color4(m_BorderColor);

                GL.Begin(PrimitiveType.LineStrip);
                GL.Vertex3(s, m_height, s);
                GL.Vertex3(-s, m_height, s);
                GL.Vertex3(-s, m_height, -s);
                GL.Vertex3(s, m_height, -s);
                GL.Vertex3(s, m_height, s);
                GL.Vertex3(s, -s, s);
                GL.Vertex3(-s, -s, s);
                GL.Vertex3(-s, -s, -s);
                GL.Vertex3(s, -s, -s);
                GL.Vertex3(s, -s, s);
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(-s, m_height, s);
                GL.Vertex3(-s, -s, s);
                GL.Vertex3(-s, m_height, -s);
                GL.Vertex3(-s, -s, -s);
                GL.Vertex3(s, m_height, -s);
                GL.Vertex3(s, -s, -s);
                GL.End();
            }
        }


        private Color m_BorderColor, m_FillColor;
        private float m_height;
    }

    class StarRenderer : ObjectRenderer
    {
        private bool m_showsStar = false;
        private NormalBMDRenderer m_ModelRenderer = null;
        private NormalBMDRenderer m_StarRenderer = null;

        public StarRenderer(LevelObject obj)
        {
            char startype = obj.Parameters[0].ToString("X4")[2];
            if (obj.ID == 63)
            {
                switch (startype)
                {
                    case '0':
                        m_ModelRenderer = new NormalBMDRenderer("data/normal_obj/star/star_base.bmd", 0.008f);
                        m_Filename = "data/normal_obj/star/star_base.bmd";
                        break;
                    case '1':
                        m_ModelRenderer = new NormalBMDRenderer("data/normal_obj/star_box/star_box.bmd", 0.008f);
                        m_Filename = "data/normal_obj/star_box/star_box.bmd";
                        break;
                    case '4':
                        m_ModelRenderer = new NormalBMDRenderer("data/normal_obj/star_box/star_box.bmd", 0.008f);
                        m_Filename = "data/normal_obj/star_box/star_box.bmd";
                        break;
                    case '6':
                        m_ModelRenderer = new NormalBMDRenderer("data/normal_obj/star_box/star_box.bmd", 0.008f);
                        m_Filename = "data/normal_obj/star_box/star_box.bmd";
                        m_showsStar = true;
                        break;
                    default:
                        m_showsStar = true;
                        break;
                }
            }
            else if (obj.ID == 61)
            {
                switch (startype)
                {
                    case '6':
                        m_ModelRenderer = new NormalBMDRenderer("data/normal_obj/star_box/star_box.bmd", 0.008f);
                        m_Filename = "data/normal_obj/star_box/star_box.bmd";
                        m_showsStar = true;
                        break;
                    default:
                        m_StarRenderer = new NormalBMDRenderer("data/normal_obj/star/obj_star.bmd", 0.008f);
                        m_Filename = "data/normal_obj/star/obj_star.bmd";
                        m_showsStar = true;
                        break;
                }
            }
            else
            {
                m_showsStar = true;
            }
        }

        public override bool GottaRender(RenderMode mode)
        {
            bool star = false;
            bool additional = false;
            if (m_showsStar)
                star = (m_StarRenderer != null) ? m_StarRenderer.GottaRender(mode) : mode != RenderMode.Opaque;
            
            additional = (m_ModelRenderer != null) ? m_ModelRenderer.GottaRender(mode) : false;
            return star || additional;
        }

        public override void Render(RenderMode mode)
        {
            if (m_showsStar)
            {
                if (m_StarRenderer != null)
                {
                    m_StarRenderer.Render(mode);
                }
                else
                {
                    const float s = 0.08f;

                    if (mode != RenderMode.Picking)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                        GL.Color4(Color.FromArgb(100, 255, 200, 0));
                        GL.Disable(EnableCap.Lighting);
                    }
                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.Vertex3(0, 0, 0.25 * s);
                    GL.Vertex3(0, s, 0);
                    for (int i = 0; i <= 5; i++)
                    {
                        GL.Vertex3(Math.Sin(i * 1.25664) * s, Math.Cos(i * 1.25664) * s, 0);
                        GL.Vertex3(Math.Sin(i * 1.25664 + 0.62832) * s * 0.5, Math.Cos(i * 1.25664 + 0.62832) * s * 0.5, 0);
                    }
                    GL.End();

                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.Vertex3(0, 0, -0.25 * s);
                    GL.Vertex3(0, s, 0);
                    for (int i = 4; i >= 0; i--)
                    {
                        GL.Vertex3(Math.Sin(i * 1.25664 + 0.62832) * s * 0.5, Math.Cos(i * 1.25664 + 0.62832) * s * 0.5, 0);
                        GL.Vertex3(Math.Sin(i * 1.25664) * s, Math.Cos(i * 1.25664) * s, 0);
                    }
                    GL.End();


                    if (mode != RenderMode.Picking)
                    {
                        GL.LineWidth(2.0f);
                        GL.Color4(Color.FromArgb(255, 200, 0));

                        GL.Begin(PrimitiveType.LineLoop);
                        GL.Vertex3(0, s, 0);
                        for (int i = 0; i < 5; i++)
                        {
                            GL.Vertex3(Math.Sin(i * 1.25664) * s, Math.Cos(i * 1.25664) * s, 0);
                            GL.Vertex3(Math.Sin(i * 1.25664 + 0.62832) * s * 0.5, Math.Cos(i * 1.25664 + 0.62832) * s * 0.5, 0);
                        }
                        GL.End();

                    }
                }
            }
            if (m_ModelRenderer != null) m_ModelRenderer.Render(mode);
        }

        public override void Release()
        {
            if (m_ModelRenderer != null) m_ModelRenderer.Release();
            if (m_StarRenderer != null) m_StarRenderer.Release();
        }
    }

    class ExitRenderer : ObjectRenderer
    {
        private float m_XScale, m_YScale, m_XRotation;

        public ExitRenderer(ushort param, ushort param2)
        {
            m_XScale = (float)(((param2 >> 8) & 0xF) + 1) * 0.1f;
            m_YScale = (float)(((param2 >> 12) & 0xF) + 1) * 0.1f;
            m_XRotation = 360.0f / 65536.0f * (float)param;

        }

        public override bool GottaRender(RenderMode mode)
        {
            if (mode == RenderMode.Opaque) return false;
            else return true;
        }

        public override void Render(RenderMode mode)
        {
            const float s = 0.02f;
            float halfWidth = m_XScale*0.5f;
            GL.Rotate(m_XRotation, 1f, 0f, 0f);

            if (mode != RenderMode.Picking)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Color4(Color.FromArgb(100, 255, 0, 0));
                GL.Disable(EnableCap.Lighting);
            }

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-halfWidth, 0, -s);
            GL.Vertex3(-halfWidth, m_YScale, -s);
            GL.Vertex3(halfWidth, 0, -s);
            GL.Vertex3(halfWidth, m_YScale, -s);
            GL.Vertex3(halfWidth, 0, s);
            GL.Vertex3(halfWidth, m_YScale, s);
            GL.Vertex3(-halfWidth, 0, s);
            GL.Vertex3(-halfWidth, m_YScale, s);
            GL.Vertex3(-halfWidth, 0, -s);
            GL.Vertex3(-halfWidth, m_YScale, -s);
            GL.End();

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-halfWidth, m_YScale, -s);
            GL.Vertex3(-halfWidth, m_YScale, s);
            GL.Vertex3(halfWidth, m_YScale, -s);
            GL.Vertex3(halfWidth, m_YScale, s);
            GL.End();

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(-halfWidth, 0, -s);
            GL.Vertex3(halfWidth, 0, -s);
            GL.Vertex3(-halfWidth, 0, s);
            GL.Vertex3(halfWidth, 0, s);
            GL.End();

            if (mode != RenderMode.Picking)
            {
                GL.LineWidth(2.0f);
                GL.Color4(Color.FromArgb(255,0,0));

                GL.Begin(PrimitiveType.LineStrip);
                GL.Vertex3(halfWidth, m_YScale, s);
                GL.Vertex3(-halfWidth, m_YScale, s);
                GL.Vertex3(-halfWidth, m_YScale, -s);
                GL.Vertex3(halfWidth, m_YScale, -s);
                GL.Vertex3(halfWidth, m_YScale, s);
                GL.Vertex3(halfWidth, 0, s);
                GL.Vertex3(-halfWidth, 0, s);
                GL.Vertex3(-halfWidth, 0, -s);
                GL.Vertex3(halfWidth, 0, -s);
                GL.Vertex3(halfWidth, 0, s);
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(-halfWidth, m_YScale, s);
                GL.Vertex3(-halfWidth, 0, s);
                GL.Vertex3(-halfWidth, s, -s);
                GL.Vertex3(-halfWidth, 0, -s);
                GL.Vertex3(halfWidth, m_YScale, -s);
                GL.Vertex3(halfWidth, 0, -s);
                GL.End();
            }
        }
    }

    class DoorRenderer : ObjectRenderer
    {
        private DoorObject m_DoorObj;
        private NormalBMDRenderer m_MainRenderer, m_AuxRenderer;

        public DoorRenderer(DoorObject obj)
        {
            m_DoorObj = obj;
            int doortype = m_DoorObj.DoorType;

            m_MainRenderer = m_AuxRenderer = null;
            if ((doortype >= 1 && doortype <= 8) || doortype == 13 || doortype == 14 || (doortype >= 19 && doortype <= 23))
            {
                m_MainRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door0.bmd", 1f);
                switch (doortype)
                {
                    case 2: m_AuxRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door0_star.bmd", 1f); break;
                    case 3: 
                    case 13: m_AuxRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door0_star1.bmd", 1f); break;
                    case 4: 
                    case 14: m_AuxRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door0_star3.bmd", 1f); break;
                    case 5: m_AuxRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door0_star10.bmd", 1f); break;
                    case 6:
                    case 7:
                    case 19:
                    case 20:
                    case 21:
                    case 22: 
                    case 23: m_AuxRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door0_keyhole.bmd", 1f); break;
                }
            }
            else if (doortype >= 9 && doortype <= 12)
            {
                m_MainRenderer = new NormalBMDRenderer("data/normal_obj/stargate/obj_stargate.bmd", 1f);
            }
            else
                switch (doortype)
                {
                    case 15: m_MainRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door2_boro.bmd", 1f); break;
                    case 16: m_MainRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door3_tetsu.bmd", 1f); break;
                    case 17: m_MainRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door4_yami.bmd", 1f); break;
                    case 18: m_MainRenderer = new NormalBMDRenderer("data/normal_obj/door/obj_door5_horror.bmd", 1f); break;
                }
        }

        public override bool GottaRender(RenderMode mode)
        {
            if (m_DoorObj.DoorType == 0) return mode != RenderMode.Translucent; // cheat to give it priority
            else
            {
                bool main = (m_MainRenderer != null) ? m_MainRenderer.GottaRender(mode) : false;
                bool aux = (m_AuxRenderer != null) ? m_AuxRenderer.GottaRender(mode) : false;
                return main || aux;
            }
        }

        public override void Render(RenderMode mode)
        {
            if (m_DoorObj.DoorType == 0)
            {
                float sx = (m_DoorObj.PlaneSizeX + 1) * 0.05f;
                float sy = (m_DoorObj.PlaneSizeY + 1) * 0.05f;

                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);

                if (mode != RenderMode.Picking)
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.Disable(EnableCap.Lighting);
                    GL.Color4(0f, 1f, 0f, 1f);

                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Vertex3(sx, 0f, 0f);
                    GL.Vertex3(sx, sy * 2f, 0f);
                    GL.Vertex3(-sx, sy * 2f, 0f);
                    GL.Vertex3(-sx, 0f, 0f);
                    GL.End();

                    GL.Color4(0.75f, 1f, 0.75f, 0.75f);
                }

                GL.Begin(PrimitiveType.Quads);
                GL.Vertex3(sx, 0f, 0f);
                GL.Vertex3(sx, sy * 2f, 0f);
                GL.Vertex3(-sx, sy * 2f, 0f);
                GL.Vertex3(-sx, 0f, 0f);
                GL.End();

                if (mode != RenderMode.Picking)
                    GL.Color4(1f, 0.75f, 0.75f, 0.75f);

                GL.Begin(PrimitiveType.Quads);
                GL.Vertex3(-sx, 0f, 0f);
                GL.Vertex3(-sx, sy * 2f, 0f);
                GL.Vertex3(sx, sy * 2f, 0f);
                GL.Vertex3(sx, 0f, 0f);
                GL.End();
            }
            else
            {
                GL.Scale(0.008f, 0.008f, 0.008f);
                m_MainRenderer.Render(mode);
                if (m_DoorObj.DoorType >= 9 && m_DoorObj.DoorType <= 12)
                {
                    GL.Rotate(180f, 0f, 1f, 0f);
                    m_MainRenderer.Render(mode);
                }
                if (m_AuxRenderer != null) m_AuxRenderer.Render(mode);
            }
        }

        public override void Release()
        {
            if (m_MainRenderer != null) m_MainRenderer.Release();
            if (m_AuxRenderer != null) m_AuxRenderer.Release();
        }
    }


    class NormalBMDRenderer : ObjectRenderer
    {
        public NormalBMDRenderer() { }
        public NormalBMDRenderer(string filename, float scale) 
        { 
            Construct(filename, scale);
        }

        public override void Release()
        {
            ModelCache.RemoveModel(m_Model);
        }

        public override bool GottaRender(RenderMode mode)
        {
            int dl = 0;
            switch (mode)
            {
                case RenderMode.Opaque: dl = m_DisplayLists[0]; break;
                case RenderMode.Translucent: dl = m_DisplayLists[1]; break;
                case RenderMode.Picking: dl = m_DisplayLists[2]; break;
            }

            return dl != 0;
        }

        public override void Render(RenderMode mode)
        {
            GL.Scale(m_Scale);
            switch (mode)
            {
                case RenderMode.Opaque: GL.CallList(m_DisplayLists[0]); break;
                case RenderMode.Translucent: GL.CallList(m_DisplayLists[1]); break;
                case RenderMode.Picking: GL.CallList(m_DisplayLists[2]); break;
            }
        }

        public void Construct(string filename, float scale)
        {
            m_Model = ModelCache.GetModel(filename);
            m_DisplayLists = ModelCache.GetDisplayLists(m_Model);
            m_Scale = new Vector3(scale, scale, scale);
            m_Filename = filename;
        }

        public override void UpdateRenderer()
        {
            ModelCache.RemoveModel(m_Model);
            Construct(m_Filename, m_Scale.X);
        }

        private BMD m_Model;
        private int[] m_DisplayLists;
    }


    class PaintingRenderer : NormalBMDRenderer
    {
        private float m_XScale, m_YScale, m_XRotation;

        private bool m_Mirror;

        public PaintingRenderer(ushort param, ushort param2)
        {
            string[] ptgnames = { "for_bh", "for_bk", "for_ki", "for_sm", "for_cv_ex5", "for_fl", "for_dl", "for_wl",
                                  "for_sl", "for_wc", "for_hm", "for_hs", "for_td_tt", "for_ct", "for_ex_mario", "for_ex_luigi",
                                  "for_ex_wario", "for_vs_cross", "for_vs_island" };
            int ptgid = (param >> 8) & 0x1F;
            if (ptgid > 18) ptgid = 18;
            string filename = "data/picture/" + ptgnames[ptgid] + ".bmd";
            m_XScale = (float)((param & 0xF) + 1) / 16f;
            m_YScale = (float)(((param >> 4) & 0xF) + 1) / 16f;
            m_Mirror = (((param >> 13) & 0x3) == 3); //no way to show it properly

            m_XRotation = 360.0f / 65536.0f*(float)param2;

            Construct(filename, 0.128f);
        }

        public override void Render(RenderMode mode)
        {
            GL.Rotate(m_XRotation, 1f, 0f, 0f);
            GL.Scale(m_XScale, m_YScale, 1f);
            GL.Translate(0f, 0.8f, 0f);
            base.Render(mode);
        }
    }

    class TreeRenderer : NormalBMDRenderer
    {
        public TreeRenderer(int treetype)
        {
            string[] treenames = { "bomb", "toge", "yuki", "yashi", "castle", "castle", "castle", "castle" };
            if (treetype > 7) treetype = 7;
            string filename = "data/normal_obj/tree/" + treenames[treetype] + "_tree.bmd";
            Construct(filename, 0.008f);
        }
    }

    class KurumajikuRenderer : ObjectRenderer
    {
        private NormalBMDRenderer m_KurumaRenderer, m_KurumajikuRenderer;

        public KurumajikuRenderer(string lvl)
        {
            m_KurumaRenderer = new NormalBMDRenderer("data/special_obj/"+lvl+"_kuruma/"+lvl+"_kuruma.bmd", 1f);
            m_KurumajikuRenderer = new NormalBMDRenderer("data/special_obj/"+lvl+"_kuruma/"+lvl+"_kurumajiku.bmd", 1f);
            this.m_Filename = m_KurumaRenderer.m_Filename + ";" + m_KurumajikuRenderer.m_Filename;
        }

        public override void Release()
        {
            m_KurumaRenderer.Release();
            m_KurumajikuRenderer.Release();
        }

        public override bool GottaRender(RenderMode mode)
        {
            return m_KurumaRenderer.GottaRender(mode) || m_KurumajikuRenderer.GottaRender(mode);
        }

        public override void Render(RenderMode mode)
        {
            GL.Scale(0.008f, 0.008f, 0.008f);
            m_KurumajikuRenderer.Render(mode);

            GL.Translate(50f, 0f, 37.5f);
            m_KurumaRenderer.Render(mode);
            GL.Translate(-50f, 50f, 0f);
            m_KurumaRenderer.Render(mode);
            GL.Translate(-50f, -50f, 0f);
            m_KurumaRenderer.Render(mode);
            GL.Translate(50f, -50f, 0f);
            m_KurumaRenderer.Render(mode);
        }
    }

    class Koopa3bgRenderer : NormalBMDRenderer
    {
        public Koopa3bgRenderer(int npart)
        {
            string partnames = "abcdefghij";
            if (npart > 9) npart = 9;
            string filename = "data/special_obj/kb3_stage/kb3_" + partnames[npart] + ".bmd";
            Construct(filename, 0.008f);
        }
    }

    class ToxboxRenderer : NormalBMDRenderer
    {
        public ToxboxRenderer()
            : base("data/enemy/onimasu/onimasu.bmd", 0.008f)
        {
        }

        public override void Render(RenderMode mode)
        {
            GL.Translate(0.0f, 0.25f, 0.0f);
            base.Render(mode);
        }
    }

    class C1TrapRenderer : NormalBMDRenderer
    {
        public C1TrapRenderer()
            : base("data/special_obj/c1_trap/c1_trap.bmd", 1f)
        {
        }

        public override void Render(RenderMode mode)
        {
            GL.Scale(0.008f, 0.008f, 0.008f);
            base.Render(mode);
            GL.Translate(-44f, 0f, 0f);
            base.Render(mode);
        }
    }

    class FlPuzzleRenderer : NormalBMDRenderer
    {
        public FlPuzzleRenderer(int npart)
        {
            if (npart > 13)
                npart = 13;
            string filename = "data/special_obj/fl_puzzle/fl_14_" + npart.ToString("D2") + ".bmd";
            Construct(filename, 0.008f);
        }
    }

    class BigSnowmanRenderer : DoubleRenderer
    {
        public BigSnowmanRenderer() : base("data/enemy/big_snowman/big_snowman_body.bmd", "data/enemy/big_snowman/big_snowman_head.bmd", 1f) { }

        public override void Render(RenderMode mode)
        {
            GL.Scale(0.012f, 0.012f, 0.012f);
            GL.Translate(0f, 5f, 0f);
            m_PrimaryRenderer.Render(mode);

            GL.Translate(0f, 25f, 0f);
            m_SecondaryRenderer.Render(mode);
        }
    }

    class PokeyRenderer : ObjectRenderer
    {
        private NormalBMDRenderer m_HeadRenderer, m_BodyRenderer;

        public PokeyRenderer()
        {
            m_HeadRenderer = new NormalBMDRenderer("data/enemy/sanbo/sanbo_head.bmd", 1f);
            m_BodyRenderer = new NormalBMDRenderer("data/enemy/sanbo/sanbo_body.bmd", 1f);
            this.m_Filename = m_HeadRenderer.m_Filename + ";" + m_BodyRenderer.m_Filename;
        }

        public override void Release()
        {
            m_HeadRenderer.Release();
            m_BodyRenderer.Release();
        }

        public override bool GottaRender(RenderMode mode)
        {
            return m_HeadRenderer.GottaRender(mode) || m_BodyRenderer.GottaRender(mode);
        }

        public override void Render(RenderMode mode)
        {
            GL.Scale(0.008f, 0.008f, 0.008f);
            GL.Translate(0f, 5f, 0f);
            m_BodyRenderer.Render(mode);
            GL.Translate(0f, 15f, 0f);
            m_BodyRenderer.Render(mode);
            GL.Translate(0f, 15f, 0f);
            m_BodyRenderer.Render(mode);
            GL.Translate(0f, 15f, 0f);
            m_BodyRenderer.Render(mode);

            GL.Translate(0f, 15f, 0f);
            m_HeadRenderer.Render(mode);
        }
    }

    class WigglerRenderer : ObjectRenderer
    {
        private NormalBMDRenderer m_HeadRenderer;
        private NormalBMDRenderer[] m_BodyRenderer = new NormalBMDRenderer[4];

        public WigglerRenderer()
        {
            m_HeadRenderer = new NormalBMDRenderer("data/enemy/hanachan/hanachan_head.bmd", 1f);
            this.m_Filename = m_HeadRenderer.m_Filename + ";";
            for (int i = 0; i < 4; i++)
            {
                string name = "data/enemy/hanachan/hanachan_body0" + (i + 1) + ".bmd";
                m_BodyRenderer[i] = new NormalBMDRenderer(name, 1f);
                this.m_Filename += name + ";";
            }
        }

        public override void Release()
        {
            m_HeadRenderer.Release();
            foreach (NormalBMDRenderer renderer in m_BodyRenderer)
                renderer.Release();
        }

        public override bool GottaRender(RenderMode mode)
        {
            return m_HeadRenderer.GottaRender(mode);
        }

        public override void Render(RenderMode mode)
        {
            GL.Scale(0.008f, 0.008f, 0.008f);

            m_HeadRenderer.Render(mode);

            foreach (NormalBMDRenderer renderer in m_BodyRenderer)
            {
                GL.Translate(0f, 0f, -15f);
                renderer.Render(mode);
            }
        }
    }

    class LooseWanWanRenderer : ObjectRenderer
    {
        private NormalBMDRenderer m_BodyRenderer, m_ChainRenderer;

        public LooseWanWanRenderer()
        {
            m_BodyRenderer = new NormalBMDRenderer("data/enemy/wanwan/wanwan.bmd", 1f);
            m_ChainRenderer = new NormalBMDRenderer("data/enemy/wanwan/chain.bmd", 1f);
            this.m_Filename = m_BodyRenderer.m_Filename + ";" + m_ChainRenderer.m_Filename;
        }

        public override void Release()
        {
            m_BodyRenderer.Release();
            m_ChainRenderer.Release();
        }

        public override bool GottaRender(RenderMode mode)
        {
            return m_BodyRenderer.GottaRender(mode) || m_ChainRenderer.GottaRender(mode);
        }

        public override void Render(RenderMode mode)
        {
            GL.PushMatrix();
            GL.Scale(0.008f, 0.008f, 0.008f);
            for (int i = 0; i < 6; i++)
            {
                GL.Translate(0f, 3.25f, 10f);
                m_ChainRenderer.Render(mode);
            }

            GL.Translate(0f, 3.25f, 40f);
            m_BodyRenderer.Render(mode);
            GL.PopMatrix();
        }
    }

    class ChainedWanWanRenderer : LooseWanWanRenderer
    {
        private NormalBMDRenderer m_PoleRenderer;

        public ChainedWanWanRenderer()
        {
            m_PoleRenderer = new NormalBMDRenderer("data/normal_obj/obj_pile/pile.bmd", 0.008f);
            this.m_Filename = base.m_Filename + ";" + m_PoleRenderer.m_Filename;
        }

        public override void Release()
        {
            base.Release();
            m_PoleRenderer.Release();
        }

        public override bool GottaRender(RenderMode mode)
        {
            return base.GottaRender(mode) || m_PoleRenderer.GottaRender(mode);
        }

        public override void Render(RenderMode mode)
        {
            GL.PushMatrix();
            m_PoleRenderer.Render(mode);
            GL.PopMatrix();

            base.Render(mode);
        }
    }

    class DoubleRenderer : ObjectRenderer
    {
        protected NormalBMDRenderer m_PrimaryRenderer, m_SecondaryRenderer;
        Vector3 m_OffsetFirst, m_OffsetSecond;
        float scale;

        public DoubleRenderer(String first, String second, Vector3 offsetFirst, Vector3 offsetSecond, float scale)
        {
            m_PrimaryRenderer = new NormalBMDRenderer(first, 1f);
            m_SecondaryRenderer = new NormalBMDRenderer(second, 1f);
            m_OffsetFirst = offsetFirst;
            m_OffsetSecond = offsetSecond;
            this.scale = scale;
            this.m_Filename = first + ";" + second;
        }

        public DoubleRenderer(String first, String second, float scale) :
            this(first, second, Vector3.Zero, Vector3.Zero, scale) { }

        public override void Release()
        {
            m_PrimaryRenderer.Release();
            m_SecondaryRenderer.Release();
        }

        public override bool GottaRender(RenderMode mode)
        {
            return m_SecondaryRenderer.GottaRender(mode) || m_PrimaryRenderer.GottaRender(mode);
        }

        public override void Render(RenderMode mode)
        {
            GL.Scale(scale, scale, scale);
            GL.Translate(m_OffsetFirst.X, m_OffsetFirst.Y, m_OffsetFirst.Z);
            m_PrimaryRenderer.Render(mode);
            GL.Translate(m_OffsetSecond.X, m_OffsetSecond.Y, m_OffsetSecond.Z);
            m_SecondaryRenderer.Render(mode);
        }
    }

    class ColourArrowRenderer : ObjectRenderer
    {
        float rotX;
        float rotY;
        float rotZ;

        public ColourArrowRenderer(Color border, Color fill, bool showaxes)
        {
            m_BorderColor = border;
            m_FillColor = fill;
            m_ShowAxes = showaxes;

            rotX = 0.0f;
            rotY = 0.0f;
            rotZ = 0.0f;
        }

        public ColourArrowRenderer(Color border, Color fill, bool showaxes, float rotX = 0, float rotY = 0, float rotZ = 0)
        {
            m_BorderColor = border;
            m_FillColor = fill;
            m_ShowAxes = showaxes;

            this.rotX = rotX;
            this.rotY = rotY;
            this.rotZ = rotZ;
        }

        public override bool GottaRender(RenderMode mode)
        {
            if (mode == RenderMode.Translucent) return false;
            else return true;
        }

        public override void Render(RenderMode mode)
        {
            const float s = 0.04f;

            if (mode != RenderMode.Picking)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Color4(m_FillColor);
                GL.Disable(EnableCap.Lighting);
            }

            GL.Rotate(rotX, Vector3d.UnitX);
            GL.Rotate(rotY, Vector3d.UnitY);
            GL.Rotate(rotZ, Vector3d.UnitZ);

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(0, -s, -s * 1.5f);
            GL.Vertex3(0, s, -s * 1.5f);
            GL.Vertex3(s, -s, s);
            GL.Vertex3(s, s, s);
            GL.Vertex3(-s, -s, s);
            GL.Vertex3(-s, s, s);
            GL.Vertex3(0, -s, -s * 1.5f);
            GL.Vertex3(0, s, -s * 1.5f);
            GL.End();

            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(0, s, -s * 1.5f);
            GL.Vertex3(-s, s, s);
            GL.Vertex3(s, s, s);
            GL.End();

            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(s, -s, s);
            GL.Vertex3(-s, -s, s);
            GL.Vertex3(0, -s, -s * 1.5f);
            GL.End();

            if (mode != RenderMode.Picking)
            {
                GL.LineWidth(3f);
                GL.Color4(m_BorderColor);

                GL.Begin(PrimitiveType.LineStrip);
                GL.Vertex3(s, s, s);
                GL.Vertex3(-s, s, s);
                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex3(0, s, -s * 1.5f);
                GL.Color4(m_BorderColor);
                GL.Vertex3(s, s, s);
                GL.Vertex3(s, -s, s);
                GL.Vertex3(-s, -s, s);
                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex3(0, -s, -s * 1.5f);
                GL.Color4(m_BorderColor);
                GL.Vertex3(s, -s, s);
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(-s, s, s);
                GL.Vertex3(-s, -s, s);
                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex3(0, s, -s * 1.5f);
                GL.Vertex3(0, -s, -s * 1.5f);
                GL.Color4(m_BorderColor);
                GL.End();

                if (m_ShowAxes)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    GL.Vertex3(0.0f, 0.0f, 0.0f);
                    GL.Color3(1.0f, 0.0f, 0.0f);
                    GL.Vertex3(s * 2.0f, 0.0f, 0.0f);
                    GL.Color3(0.0f, 1.0f, 0.0f);
                    GL.Vertex3(0.0f, 0.0f, 0.0f);
                    GL.Color3(0.0f, 1.0f, 0.0f);
                    GL.Vertex3(0.0f, s * 2.0f, 0.0f);
                    GL.Color3(0.0f, 0.0f, 1.0f);
                    GL.Vertex3(0.0f, 0.0f, 0.0f);
                    GL.Color3(0.0f, 0.0f, 1.0f);
                    GL.Vertex3(0.0f, 0.0f, s * 2.0f);
                    GL.End();
                }
            }
        }

        private Color m_BorderColor, m_FillColor;
        private bool m_ShowAxes;
    }
}
