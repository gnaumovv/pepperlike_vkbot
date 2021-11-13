using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;


namespace pepperlike_vkbot
{
    class VKAPISession
    {
        VkApi vk_session = new VkApi();

        public const ulong appId = 7895051;
        string token = "token"; // Вставить свой access_token

        public bool VkAuth()
        {
            try
            {
                vk_session.Authorize(new ApiAuthParams
                {
                    AccessToken = token
                });

                if (vk_session.Account.GetProfileInfo().GetType() != null) return true; // Проверка авторизации под пользователем
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nAUTHORIZATION ERROR:\n" + e);
                return false;
            }
        }

        public WallGetObject GetWall(string public_domain) // Получаем стену впервые
        {
            return vk_session.Wall.Get(new WallGetParams { Domain = public_domain });
        }

        public WallGetObject GetWall(string public_domain, ulong count, ulong offset) // Получаем диапазон постов со стены
        {
            return vk_session.Wall.Get(new WallGetParams { Domain = public_domain, Count = count, Offset = offset });
        }

        public void Like(long id, long? owner_id) // Ставит Like На одну запись
        {
            vk_session.Likes.Add(new LikesAddParams
            {
                Type = LikeObjectType.Post,
                ItemId = id,
                OwnerId = owner_id,
                AccessKey = vk_session.Token
            });
        }
    }
}
