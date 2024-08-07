using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubFetch
{
    class GitHubFetch
    {

        public static async Task<Dictionary<char, long>> getStatistics(string owner, string repo, string headerName, string access_token = null)
        {
            var client = new GitHubClient(new ProductHeaderValue(headerName));

            if (access_token != null)
            {
                client.Credentials = new Credentials(access_token);
            }


            Dictionary<char, long> statistics = new Dictionary<char, long>
            {
                ['a'] = 0,
                ['b'] = 0,
                ['c'] = 0,
                ['d'] = 0,
                ['e'] = 0,
                ['f'] = 0,
                ['g'] = 0,
                ['h'] = 0,
                ['i'] = 0,
                ['j'] = 0,
                ['k'] = 0,
                ['l'] = 0,
                ['m'] = 0,
                ['n'] = 0,
                ['o'] = 0,
                ['p'] = 0,
                ['q'] = 0,
                ['r'] = 0,
                ['s'] = 0,
                ['t'] = 0,
                ['u'] = 0,
                ['v'] = 0,
                ['w'] = 0,
                ['x'] = 0,
                ['y'] = 0,
                ['z'] = 0
            };

            string index = string.Concat(statistics.Select(kvp => kvp.Key));

            try
            {
                return readDir(client, owner, repo, statistics, index).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private static async Task<Dictionary<char, long>> readDir(GitHubClient client, string owner, string repo, Dictionary<char, long> statistics, string index, string path = null)
        {
            IReadOnlyList<RepositoryContent> result;

            try
            {


                if (path == null)
                {
                    result = await client.Repository.Content.GetAllContents(owner, repo);
                }
                else
                {
                    result = await client.Repository.Content.GetAllContents(owner, repo, path);
                }

                foreach (var item in result)
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