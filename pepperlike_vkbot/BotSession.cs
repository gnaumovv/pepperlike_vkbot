using System;
using System.IO;
using System.Collections.Generic;
using VkNet.Model;
using System.Threading;

namespace pepperlike_vkbot
{
    class BotSession
    {
        private List<Post> P;
        private StreamWriter TxtWrite = new StreamWriter("history.txt", true);
        private string public_domain; // Домен группы ВК

        VKAPISession vkAPISession = new VKAPISession();

        public bool session()
        {
            if (vkAPISession.VkAuth())
            {
                Console.WriteLine("Authorization completed successfully");

                while (true) // Цикл работы с одной группой
                {
                    Console.Write("\nPublic Domain: "); public_domain = Console.ReadLine();
                    Console.WriteLine($"\nGetting all posts from [{public_domain}]");
                    P = AllPostsStatus();

                    int c = 0; // Счётчик постов с "Like"
                    int countToLike = 0;

                    foreach (Post p in P) // Данные постов записываем в лог и в консоль
                    {
                        if (p.isLiked) c++;
                        p.Write(TxtWrite);
                    }

                    Console.Write($"\nLiked count: {c}\n");

                    while (true)
                    {
                        Console.Write("\nCount to like: ");
                        countToLike = Int32.Parse(Console.ReadLine());
                        if (countToLike + c <= P.Count) break; // Нельзя лайкнуть записей больше, чем есть не лайкнутых
                    }

                    LikeByID(P, countToLike);

                    Console.WriteLine($"\nResult:");
                    P = AllPostsStatus();
                    foreach (Post p in P) p.Write(TxtWrite);

                    Console.Write("\nStart a new session? (1 / 0): "); if (Int32.Parse(Console.ReadLine()) != 1) break;
                }
                Console.WriteLine("\nThe End \n\t (0__o)\n\n");
                return true;
            }
            else
            {
                Console.WriteLine("\nINVALID ACCESS_TOKEN...\nGoodBye!");
                return false;
            }

        }

        private List<Post> AllPostsStatus() // Получение данных каждого поста со стены группы
        {
            List<Post> Posts = new List<Post>();

            WallGetObject wall = vkAPISession.GetWall(public_domain); // Первый запрос к стене, получаем общее количество записей

            Console.WriteLine($"\n Post's Total count: {wall.TotalCount}\n");

            ulong totalCount = wall.TotalCount; // Вводим ограничение, т.к. метод API позволяет за раз выгрузить только 100 записей

            int i = 1;
            int request_count = 1;
            ulong takenPostsCount = 0;

            while (takenPostsCount < totalCount)
            {
                Console.Write($"Запрос №{request_count++} \t {DateTime.Now}");
                Console.WriteLine($" - {DateTime.Now}");

                wall = vkAPISession.GetWall(public_domain, totalCount > 100 ? 100 : totalCount, takenPostsCount);

                Thread.Sleep(300);

                foreach (var W in wall.WallPosts) // Добавляем посты в локальный список
                {
                    Post P = new Post(i, (long)W.Id, W.Likes.UserLikes, (long)W.FromId);
                    Posts.Add(P);
                    i++;
                }

                takenPostsCount += (ulong)wall.WallPosts.Count;
            }

            Posts.Reverse(); // Переворачиваем: первыми идут самые свежие посты
            return Posts;
        }

        private void LikeByID(List<Post> P, int count) // Like посту по его ID
        {
            Random R = new Random();
            int likeDelay; // Задержка между Like (защита от Flood Control)
            int i = 0;

            for (int c = 1; c <= count;)
            {
                if (!P[i].isLiked)
                {
                    likeDelay = R.Next(45000, 70000);
                    Thread.Sleep(likeDelay);
                    vkAPISession.Like(P[i].id, P[i].ownerId);
                    Console.WriteLine($"№{P[i].number} \t PostID: {P[i].id} \t GET LIKE \t {DateTime.Now} (InputDelay: {((double)likeDelay / 1000)}s)");
                    c++;
                }
                i++;
            }
        }

    }
}
