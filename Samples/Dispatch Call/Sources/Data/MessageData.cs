/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Game.Sample.DispatchCall
{
    /// <summary>
    /// 网络协议码定义
    /// </summary>
    public static class ProtoOpcode
    {
        public const ushort EnterWorldResp = 101;
        public const ushort LeaveWorldResp = 102;
        public const ushort LevelSpawnResp = 103;

        public const ushort LevelUpgradeResp = 111;
        public const ushort ActorHurtResp = 112;
        public const ushort ActorDieResp = 113;

        public const ushort ActorChatResp = 211;
    }

    /// <summary>
    /// 基础信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class BaseInfo : ProtoBuf.Extension.Object
    {
        [ProtoBuf.ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public string Name { get; set; }
    }

    /// <summary>
    /// 属性状态信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class AttrStatInfo : ProtoBuf.Extension.Object
    {
        /// <summary>
        /// 等级
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public int Level { get; set; }

        /// <summary>
        /// 经验
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public int Exp { get; set; }

        /// <summary>
        /// 生命
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public int Health { get; set; }

        /// <summary>
        /// 体力
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public int Energy { get; set; }

        /// <summary>
        /// 攻击
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public int Attack { get; set; }
    }

    /// <summary>
    /// 三维向量信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class Vector3Info : ProtoBuf.Extension.Object
    {
        [ProtoBuf.ProtoMember(1)]
        public float x { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public float y { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public float z { get; set; }
    }

    /// <summary>
    /// 技能信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class SkillInfo : ProtoBuf.Extension.Object
    {
        [ProtoBuf.ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        /// <summary>
        /// 攻击范围
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public float Range { get; set; }

        /// <summary>
        /// 冷却时间
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public float CoolingTime { get; set; }
    }

    /// <summary>
    /// 士兵信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class SoldierInfo : ProtoBuf.Extension.Object
    {
        [ProtoBuf.ProtoMember(1)]
        public BaseInfo Basic { get; set; }

        /// <summary>
        /// 属性状态
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public AttrStatInfo AttrStat { get; set; }

        /// <summary>
        /// 技能列表
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public List<SkillInfo> SkillList { get; set; } = new();

        /// <summary>
        /// 位置
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public Vector3Info Position { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public Vector3Info Direction { get; set; }
    }

    /// <summary>
    /// 玩家信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class PlayerInfo : ProtoBuf.Extension.Object
    {
        [ProtoBuf.ProtoMember(1)]
        public SoldierInfo Soldier { get; set; }
    }

    /// <summary>
    /// 怪物信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class MonsterInfo : ProtoBuf.Extension.Object
    {
        [ProtoBuf.ProtoMember(1)]
        public SoldierInfo Soldier { get; set; }
    }

    /// <summary>
    /// 进入游戏回复
    /// </summary>
    [ProtoBuf.ProtoContract]
    [ProtoBuf.Extension.Message(ProtoOpcode.EnterWorldResp)]
    public partial class EnterWorldResp : ProtoBuf.Extension.Object, ProtoBuf.Extension.IMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 玩家信息
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public PlayerInfo Player { get; set; }
    }

    /// <summary>
    /// 离开游戏回复
    /// </summary>
    [ProtoBuf.ProtoContract]
    [ProtoBuf.Extension.Message(ProtoOpcode.LeaveWorldResp)]
    public partial class LeaveWorldResp : ProtoBuf.Extension.Object, ProtoBuf.Extension.IMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public int Code { get; set; }
    }

    /// <summary>
    /// 关卡刷怪回复
    /// </summary>
    [ProtoBuf.ProtoContract]
    [ProtoBuf.Extension.Message(ProtoOpcode.LevelSpawnResp)]
    public partial class LevelSpawnResp : ProtoBuf.Extension.Object, ProtoBuf.Extension.IMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public int Code { get; set; }

        /// <summary>
        /// 怪物信息
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public List<MonsterInfo> MonsterList { get; set; } = new();
    }

    /// <summary>
    /// 玩家升级回复
    /// </summary>
    [ProtoBuf.ProtoContract]
    [ProtoBuf.Extension.Message(ProtoOpcode.LevelUpgradeResp)]
    public partial class LevelUpgradeResp : ProtoBuf.Extension.Object, ProtoBuf.Extension.IMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 属性状态
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public AttrStatInfo AttrStat { get; set; }
    }

    /// <summary>
    /// 角色受击回复
    /// </summary>
    [ProtoBuf.ProtoContract]
    [ProtoBuf.Extension.Message(ProtoOpcode.ActorHurtResp)]
    public partial class ActorHurtResp : ProtoBuf.Extension.Object, ProtoBuf.Extension.IMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 伤害
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public int Damage { get; set; }
    }

    /// <summary>
    /// 角色死亡回复
    /// </summary>
    [ProtoBuf.ProtoContract]
    [ProtoBuf.Extension.Message(ProtoOpcode.ActorDieResp)]
    public partial class ActorDieResp : ProtoBuf.Extension.Object, ProtoBuf.Extension.IMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public int Uid { get; set; }
    }

    /// <summary>
    /// 聊天信息
    /// </summary>
    [ProtoBuf.ProtoContract]
    public partial class ChatInfo : ProtoBuf.Extension.Object
    {
        [ProtoBuf.ProtoMember(1)]
        public int Uid { get; set; }

        /// <summary>
        /// 聊天内容
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public string Text { get; set; }
    }

    /// <summary>
    /// 聊天信息回复
    /// </summary>
    [ProtoBuf.ProtoContract]
    [ProtoBuf.Extension.Message(ProtoOpcode.ActorChatResp)]
    public partial class ActorChatResp : ProtoBuf.Extension.Object, ProtoBuf.Extension.IMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public List<ChatInfo> ChatList { get; set; }
    }
}
