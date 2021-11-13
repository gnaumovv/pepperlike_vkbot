using System;
using System.IO;

namespace pepperlike_vkbot
{
    class Post // Структура поста для List-а постов со стены
    {
        public long id;
        public int number; // Порядковый номер поста на стене
        public bool isLiked;
        public long? ownerId;
        public Post(int number, long id, bool isLiked, long ownerId)
        {
            this.number = number;
            this.id = id;
            this.isLiked = isLiked;
            this.ownerId = ownerId;
        }

        public override string ToString()
        {
            return isLiked ? $"№{number} \t PostID: {id} \t Liked: YEEES \t {DateTime.Now}" : $"№{number} \t PostID: {id} \t Liked: NO \t {DateTime.Now}";
        }

        public void Write(StreamWriter W) // Ведём журнал в консоли и дублируем в history.txt
        {
            Console.WriteLine(this);
            W.WriteLine(this);
        }
    }
}
