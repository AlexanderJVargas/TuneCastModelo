using Newtonsoft.Json;
using System.Text;

namespace TuneCastAPIConsumer
{
    public static class Crud<T>
    {
        public static string EndPoint { get; set; }

        public static List<T> GetAll()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(EndPoint).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static T GetById(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"{EndPoint}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        //public static List<T> GetBy(String campo, int id)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var response = client.GetAsync($"{EndPoint}/{campo}/{id}").Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var json = response.Content.ReadAsStringAsync().Result;
        //            return JsonConvert.DeserializeObject<List<T>>(json);
        //        }
        //        else
        //        {
        //            throw new Exception($"Error: {response.StatusCode}");
        //        }
        //    }
        //}

        //public static T Create(T item)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var response = client.PostAsync(
        //                EndPoint,
        //                new StringContent(
        //                    JsonConvert.SerializeObject(item),
        //                    Encoding.UTF8,
        //                    "application/json"
        //                )
        //            ).Result;

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var json = response.Content.ReadAsStringAsync().Result;
        //            return JsonConvert.DeserializeObject<T>(json);
        //        }
        //        else
        //        {
        //            throw new Exception($"Error: {response.StatusCode}");
        //        }
        //    }
        //}
        public static async Task<T> Create(T item)
        {
            using (var client = new HttpClient())
            {
                // Enviar la solicitud POST de manera asincrónica
                var response = await client.PostAsync(
                    EndPoint,
                    new StringContent(
                        JsonConvert.SerializeObject(item), // Serialización a JSON
                        Encoding.UTF8,
                        "application/json"
                    )
                );

                // Verificar si la respuesta es exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer la respuesta como string de manera asincrónica
                    var json = await response.Content.ReadAsStringAsync();

                    // Deserializar la respuesta JSON y devolver el resultado
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    // Manejar errores (como el código de estado no exitoso)
                    throw new Exception($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                }
            }
        }


        public static bool Update(int id, T item)
        {
            using (var client = new HttpClient())
            {
                var response = client.PutAsync(
                        $"{EndPoint}/{id}",
                        new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static bool Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync($"{EndPoint}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }
    }
}
