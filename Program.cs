

using ConsumirAPI_JWT_Token;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

Console.WriteLine("JWT Token Validation");

int op = 0;
string BaseUrl = "http://localhost:5291/";
string token = "";

do
{
    Console.WriteLine(" ");
    Console.WriteLine("---------------------------------");
    Console.WriteLine("Informe a opção desejada:");
    Console.WriteLine("1 - para solicitar token");
    Console.WriteLine("2 - para consultar");
    Console.WriteLine("3 - para cadastrar");
    Console.WriteLine("0 - FECHAR");

    op = int.Parse(Console.ReadLine());

    Console.WriteLine(" ");

    Usuario user = new Usuario();

    switch (op)
    {
        case 0:
            break;
        case 1:
            Console.WriteLine("Informe o usuário:");
            user.Username = Console.ReadLine();
            Console.WriteLine("Informe a senha:");
            user.Password = Console.ReadLine();

            HttpClient clientToken = new HttpClient();
            clientToken.DefaultRequestHeaders.Accept.Clear();
            clientToken.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage respToken = clientToken.PostAsJsonAsync(BaseUrl + "api/autenticar", user).Result;

            if (respToken.StatusCode == HttpStatusCode.OK)
            {
                token = respToken.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Token: " + token);
            }
            else
            {
                Console.WriteLine("Retorno: " + respToken.StatusCode);
            }
            break;
        case 2:
            List<Pessoa>? pessoas = new List<Pessoa>();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
               new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync("api/pessoas");

            if (response.IsSuccessStatusCode)
            {
                var dados = response.Content.ReadAsStringAsync().Result;
                pessoas = JsonConvert.DeserializeObject<List<Pessoa>>(dados);
            }
            else
            {
                Console.WriteLine("ERRO: " + response.StatusCode.ToString());
            }

            foreach (Pessoa pessoa in pessoas)
            {
                Console.WriteLine(pessoa.id + " - " + pessoa.nome);
            }

            break;
        case 3:
            Pessoa p = new Pessoa();
            Console.WriteLine("Informe o nome da pessoa:");
            p.nome = Console.ReadLine();

            try
            {
                HttpClient clientPost = new HttpClient();
                clientPost.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

                HttpResponseMessage postResponse = await
                    clientPost.PostAsJsonAsync(BaseUrl + "api/pessoas", p);

                Console.WriteLine("Retorno: " + postResponse.StatusCode);

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
                throw;
            }

            break;
        default:
            break;
    }

} while (op != 0);