using System;

/// <summary>
/// FirebaseUser에서 받아온 사용자 데이터를 가공. 
/// 프로필에 사용.
/// </summary>
[Serializable]
public class UserInfo
{
    public string UserName;
    public string UserId;
    public string ProfileImgUrl;

    public UserInfo(string userName, string userId, string profileImgUrl = "need profileImgURL")
    {
        UserName = userName;
        UserId = userId;
        ProfileImgUrl = profileImgUrl;
    }
}
