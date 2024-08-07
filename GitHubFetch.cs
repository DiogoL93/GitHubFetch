using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubFetch
{
    public class GitHubFetch
    {

        public static async Task<Dictionary<char, long>> getStatistics(string owner, string repo, string headerName, string searchIndex, string access_token = null)
        {
            var client = new GitHubClient(new ProductHeaderValue(headerName));

            if (access_token != null && !access_token.Equals(string.Empty))
            {
                client.Credentials = new Credentials(access_token);
            }

            Dictionary<char, long> statistics = new Dictionary<char, long>();

            foreach (var item in searchIndex)
            {
                statistics.Add(item, 0);
            }

            try
            {
                var result = await readDir(client, owner, repo, statistics, searchIndex);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<Dictionary<char, long>> readDir(GitHubClient client, string owner, string repo, Dictionary<char, long> statistics, string index, string path = null)
        {
            IReadOnlyList<RepositoryContent> results;

            try
            {


                if (path == null)
                {
                    results = await client.Repository.Content.GetAllContents(owner, repo);
                }
                else
                {
                    results = await client.Repository.Content.GetAllContents(owner, repo, path);
                }

                foreach (var item in results)
                {
                    string type = item.Type.StringValue;
                    if (type.Equals(Constants.CONTENT_TYPE_DIR))
                    {
                        statistics = readDir(client, owner, repo, statistics, index, item.Path).Result;
                        Console.WriteLine(Constants.DIRECTORY_DONE + item.Name);
                    }
                    else
                    {
                        string name = item.Name;
                        if (name.EndsWith(Constants.FILE_TYPE_TS) || name.EndsWith(Constants.FILE_TYPE_JS))
                        {
                            statistics = readFile(client, owner, repo, statistics, index, item.Path).Result;
                            Console.WriteLine(Constants.FILE_DONE + name);
                        }
                    }
                }

                return statistics;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static async Task<Dictionary<char, long>> readFile(GitHubClient client, string owner, string repo, Dictionary<char, long> statistics, string index, string path)
        {
            try
            {

                IReadOnlyList<RepositoryContent> fileContent = await client.Repository.Content.GetAllContents(owner, repo, path);

                string content = fileContent[0].Content;

                foreach (var letter in index)
                {
                    statistics[letter] += content.Count(character => character == letter);
                }

                return statistics;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}