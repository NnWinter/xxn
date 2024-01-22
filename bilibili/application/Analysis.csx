using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

struct Reply{
    public long reply_id; // rpid
    public long video_id; // oid
    public int type; // 视频投稿 = 1; 文字动态 = 17; 带图动态 = 11 ;参考 作者：静弥seiya https://www.bilibili.com/read/cv8325021/ 出处：bilibili
    public long member_id; // mid
    public long root;
    public long parent;
    public long count; // reply count?
    public long rcount; // reply count?
    public DateTime time_zh; // 中国时间
    public long like; // likes, 点赞数
    public Member member; // 用户信息
    public string message; // 评论的文本
    public Reply[] replies; // 嵌套的评论

    public struct Member{
        public long id;
        public string username;
        public enum Sex{ Male, Female, Unknown }
        public Sex sex;
        public int level;
        public int officialType;
        public string officialDescription;
        public int vipType;
        public DateTime? vipDueDate;

        public Member(JObject member)
        {
            id = member["mid"].Value<long>();
            username = member["uname"].Value<string>();
            var sexstr = member["sex"].Value<string>();
            sex = sexstr == "男" ? Sex.Male : sexstr == "女" ? Sex.Female : Sex.Unknown;
            level = member["level_info"]["current_level"].Value<int>();
            officialType = member["official_verify"]["type"].Value<int>();
            officialDescription = member["official_verify"]["desc"].Value<string>();
            vipType = member["vip"]["vipType"].Value<int>();
            vipDueDate = FromUnixTime(member["vip"]["vipDueDate"].Value<long>()/1000);
        }
    }
    public Reply(JObject reply){
        reply_id = reply["rpid"].Value<long>();
        video_id = reply["oid"].Value<long>();
        type = reply["type"].Value<int>();
        member_id = reply["mid"].Value<long>();
        root = reply["root"].Value<long>();
        parent = reply["parent"].Value<long>();
        count = reply["count"].Value<long>();
        rcount = reply["rcount"].Value<long>();
        time_zh = FromUnixTime(reply["ctime"].Value<long>());
        like = reply["like"].Value<long>();
        member = new Member(reply["member"].Value<JObject>());
        message = reply["content"]["message"].Value<string>();
        if(reply["replies"].HasValues){
            replies = reply["replies"].Value<JArray>().Children<JObject>().Select(x=>new Reply(x)).ToArray();}
    }

    public static DateTime FromUnixTime(long unix){
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds( unix ).ToLocalTime();
    }
}