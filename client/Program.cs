﻿using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class WebSocketClient
{
    private ClientWebSocket _clientWebSocket;

    public WebSocketClient()
    {
        _clientWebSocket = new ClientWebSocket();
    }

    public async Task Connect(string uri)
    {
        await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        Console.WriteLine("Connected to WebSocket server.");
    }

    public async Task SendEvent(string eventType, string eventData)
    {
        var message = $"{eventType}/{eventData}";
        await SendMessage(message);
    }

    private async Task SendMessage(string message)
    {
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine($"Sent message: {message}");
    }

    public async Task Receive()
    {
        var buffer = new byte[1024];
        while (_clientWebSocket.State == WebSocketState.Open)
        {
            var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message: {receivedMessage}");
        }
    }

    public async Task Disconnect()
    {
        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected.", CancellationToken.None);
        Console.WriteLine("Disconnected from WebSocket server.");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var client = new WebSocketClient();
        await client.Connect("ws://localhost:8080");


        // "imagemessage/nicolas$UklGRhgTAABXRUJQVlA4IAwTAAAwkACdASrvAfQBPlEokUajoiIjIZHIyHAKCWlu9tVo7hgsmxS2ZRPGwiZstordc97avj9s7jW6P/b/uZza4lnzH8P51v5fxd+NuUXf7bV+gX7Mfe/OJmy+E/YA4jbyz1Nv75/2PVU0e/Vv7S/A1/Nv731vQxzIRWBa/ICqi7ZkIrAtfkBVRdsyEVgWvyAqou2ZCKwLX5AVUXbMhFYFr8gKqLtmQebvGU/VRnM31jnITI9yhxCWvyAqou2ZCKwLIiW6GeyCucoMPI8UpK+X9D4w/HmoNGgJlOEwulJYu7HT+BCOS9l4IhA35AVUXbMhFYFrVH9l/y0tkrZHDSjeuo/7s03qjMJNsPyXNy0uMcOvEYBqldfIOwiptaEDkwiNS7MhFYFr8gKnk8d0jzoHko4btM8+spi8aWyBKxLgtCZK8DEnGlM/gQyRUA6u9oeetYm0K+gSH9W7hrF6b8gKqLtmQisCyJvBF9gpbDN5Mp0XVt9BtgQvlpwJb9NxgbSJKvRGHpjCd+vZ5P/A/Wb9ijWTfkBVRdsyEVgWRJbwdJtjh+3Sb/vaE4eYdEBFVQh45Qai1Yy20wT9XrpgAqEQNNGGqQJuD2dR3izOUgKqLtmQisCyJ2cckG8AzQYBBNh/yiFItdvMKNtTxFFudSTGNaYkoEPOy+KNYug2XZkIrAtfkBVRdpYqNPhgT1DLDlUOmmvlKcx+QMZzNPsDOFEsMqOx2s4E47D2JO0fe/vDmsC1+QFVF0U6tyj26daWX5XHYW3IyJca1MlvxJDxDPvQHmHcG4aIiWDePytY7oeBICqi7ZkIrAtfiO5jHGSgPmMsmDf0YaSBaBDr8gKqLtmQisC180u3YvsdhpVSSVSPYNeF8OKPzCqi7ZkIrAtfkBRhTkd5TL2wnh3FWRUn706jvEASAZ5oveiwLX5AVUXbMhFX1kco4IFONo8aQau2dtJj+hIJAVUXbMhFYFr8fHN+2ppE1zeAUUZVvD1r8gKqLtmQisCyVlcmoEQdMIHS/90zrAREzBXFetfkBVRdsyEVgWV1JZozK3VhpAH5/nIW24eL0yEVgWvyAqou2Rn3KmWbQm6HypuuVy66SqixTaeQT1r8gKqLtmQisCIMezQcoYHSE61j6Dt8YBXexyOP0p9Fo/tK9WBa/ICqi7ZkIq+jyScQzR47RbOTHma+HpnU1TXDKGQVeV41LsyEVgWvyAqorDChWvm1n8pir5bvyBveC0GRsATTA5rAtfkBVRdsx+XM0jkIhhfCnhl39hBh2ybWYadlHDuo61+QFVF2zIRWBZKhzlmy6ZMBQyvofHv/0IFJC9Gf0Wl2ZCKwLX5AVUVhd6h+9nyzUtpsSzQkDwHhvOLsNGRRCKwLX5AVUXbMfUMTXdbO27+wGTWptJtZqeFPyK9H0/Fp6LjD9UMKqLtmQisC1+Pi/Zoyimy18NaeifOo/CisW0/UKMifsWGuU9a/ICqi7ZkIrAtfkCaUuzIRWBa/ICqi7ZkIrAtfkBVRdsyEVgWvyAqou2ZCKwLX5AVUXbMhFYFr8gKqLtKAAP7/5N8AAAAAAjNPDpNXz+Z1PE8RPk3jxH+pB8m0N2q1XJ1Es5ut1hsklSYoW1meowcAJ0cV6ukC9DIUHfqdjQPDIa73+EjJ0TId3JFXn7HuVO/QpYUrwAPMSrt300k4v2Km+u/0XSvFdzPvsOP4719Jx3AQ3eAaFJMa/6/mslyZahKw5ADzYyI+PC7qFRzTuIajTS/mUgJ6bJIfCb7h+jD93Fo62A7OgOzqlj466FI5Xo5mbyRVm570u46SaLTF6ozwJ9w1UgQlV9+fTVBAVsINrCOWBNuPtQORUTmSD6c0sqylWLs3ftxRycQeNcKIIfrk+oYmzNzItdaILm8nPJh6gW/Ug+FlJO0N8gx87bU7AIryOyu5BXuh9T9ceijh13StMHZqOUFCUphDRJfJBVsJMTSwRs+KQNX4Pm/RX1T7Nz5TK9ExVJnugnh99fVnXtotskCZjkSfSMZ3m9yWXGgptGQv9TqQai6KJaqDoMsX4KMqNl3I49Z2blPip9lcLVh4lw7+sPSL1YYXyuZO6VqPU7FO3jnaOeyCD04e0YvTpnFETuGPg3lF3TofEO0e0QDbAlAO/QIAhFjYOo+edRdxcK+JW50moEwoUQhpRbTqR3cPe+/Er+XwPTmuEjtOGjqALeQe/HPwSCfKFF8ii9LuKLt5p2/exw9h4AEfXcyo9qn0+P/T7oCTinlcyOe5o10wUJCfn3NmzDuWvWbwu50SdQGPKL1i9TM/tO7T8Ri6IRh7ZJLyKXWlGSyCqsD15clqOymdqgwDuh6SFbshmnxCMLdo5kh27ydSEwIJ2ilT7fVydAgqzOtmBmp+0UT0Q9pQ95ThzMuWZv0lmhZSsOBhPwQKW9o/O5PdpbVpGELjhpAAM/EX7UoxBdW6lgwFACgtg8eGVVOq/JouY2v2pbgac2H9QbWE8UTxfBpjK7/6uppzi/txYlXX1VCp9FmRNIsRs8d7OD4BPkZCdr8dQ02ohXWAsr+y4g9ptPdSP4zod88GTc+fzzej/LPJJf4Qxf8/hqHIv9/77YYOxITlHLU3MQO6Wn6DTi//8qAY/HF4Z5jMIAycjJk/Eu/Oe1ZcS5cE4U6S3Gao4iJ/pduQSsBnVP9IH0/C5T5tsS5qTeeKuCS7v7KO+075t6FvpXsvduz2uieQBR/j3vNpZq01VIDK3SDgBNm6OYSp3VWIMEJdAsIM2PA3og+jedmgzboQphZuVLDrzwB531JciAEGypsNha8Ejlov7owQ4jOQbRz3ZwLXnKtp+QNXFMnyMfKJefEcqaB5t/LyN9fdwNQH8eXjRnh67tuTvrtibmdfpba/rMISRvHrV5/hUbRaCmCSHd5zhlGRDURta17IWoXrTR90P/q2yW3ZeT4OPIOn5D3//nnICj67kOTKDvxV8Bb+vFrr//8X6yDmFJoQOe4boToPRqT2e08aZ87AcOZotMhLG1aEwguTZko8XpTzeRzFPfukdVvxrcUu2fJSP2Rd0MbnEWtsZlVI3t3vGwre6VlyeSSipMCpOsdeEnQb7kj/4nzWlutB8HlG/dl8TGjvJGRP6BJephZ7UwcOrF+HkN2mLCGivUKHBAS9SHxZe5pmwmKoZQeIOEjaSvb9kRZ306JSo3fTIOzHj/SNdbe1SF2TuDlltt+OgJ7sWSupbUOG1j+w/b47/afI+L5JgzBo98bKKaQuBYbGM96MZ3vo2IMn9Af5KUBHAol3Qr6XSKFA3/wZUaUfoa4b2Ro9F53psOtAIYFo/zsU79nCVm5XEpbdprHt4c/nV8WEybFJvw3Yvfn42T2ylK4XM0zYWRQhIiFWc5PqRbgkpy8EhRIHe6g/YFTNSl8YA1Kpc7rEhtEmBCQYYC/BRP0AQzjYXwvRwT5gjbUdbt/4KoDlMXx4pZM/oHu9jZ7WF1TuMsOyA13L1ycVBmXH6iG9oC19TEQxK6efigYKDmvAz95hDJf9kxys8w32H0lfY6LgOjA7aufiPsVrZSQuMuI3JKpd/z4Svx5DyN/CnsM6671gWvSIpb3Q6BN8u6khScMHM7usY9ceEQNVuiJWhKWeJ+vFa3Glk59eIbiwXR60cIFqSPAYySpR/0pTja9RulG56ZExK7c4R/DcyViI1aepi1I/jKe/f5H2oPGpmrKqGHYT2x0ZagupzoePzoFU3YQYzZEMbia9m2x7koVXT7EQjJ+ZvOojsWKMqKV6coi2bmlJ841sDbN9VbhLf60Q24ravOuShtuoHd7WXvicnLn7eanGijSlgquLzvac3TcF9y+0izi/buzOw7TBkeUw/ZXJinuJzNbqBfKV9ro3uID06qd00SnvqiraDALWQoFE9H8rvvS/WbKP6sDFqFUp0uAGSJsz0PyidTpo/sftGg0e9or3aZqcc2oi1eJ4UgDPQMXX+MS8IUdP+RnvGRUyQnssEavGNMD+ousj6/CLNFH8dISTHkbjuIsmTmqrdkxbR5Vvlv82ZF/KeNOgqOZ8j9fAHB2J7UpQ/khQ3I+zcJJ/oRZIxKncyr5y1HC8eh7kokvfKvdiNd4dOXebN35crWYrJbvCp2N9m0dDsNoiHafF96ipKPtyyX2AGoyNw/U/Q4D/1kZJ23nS3Goe5Ohn8QOkj6zzZbJvcRATw7whsfJAd/PsDRjbrmiNyvaJww85NFcCJZh23lCXmjaZp9et595qq9aw06HfvqOzQvY6TUsnwgXNmZb7k4G9xrb47MiV6+JqncPwsF95ABciIB+gAJVKeH5GKMmUQgqAAFxHLC/gH4VzT91Jlml/9V3W3ms7xr5/7Wv0CvFauNYGw7laV+aytaGl03gfsFsmA40V9qTMcNgyfJ5AONX2oSC6ctWXpRzifkt3d8nFSnVFVHH5IaOBkRwFYKON65MjOQQArWEMGoyRVYY18ZH0ukOMyJHVmbn3Td1lNQvp8IEXaKyDRyYx8izrawOLbpZmrI5qbE2uKmr0jLNQHMHNOx6+ElsMqQoE97T+m/t2TuaAGeqz11HgB52IMKKx7Af7Xc5H7X3gyJ2T0AV4F/m6U07FhT4nhT19oajzD605lWifqDdo/9OltUgwX+7xzNtWwGihUVVEiFxsTWR2ICSBNUdMNuxzoMm2986Ijvm00BVXZ6yCuHx34GG0iq5JWZPZAU2lJVOTcUQh8sR3BrtCFTF3cI+1hfBtB+i3tJWZ7110WvJXC8cvohJkXRrmk2F2G9tMVxUNZT/dxzaq/jE4fCIMiFhy5CLuR8D52sufU+NYKCHFJ4Fz6CO3H6RfqfFYNH4E7p7UaxK6toZd03OM7dSpZkgrfJvGRn/rNPAZQKA3CdC1SvwPHY+Pie06Da5A/jLiKcYgVKKiN+XBE4Fl2R7vhPgBP5/yWTvT/yAM/llmx8uyDdcDOu6x92CUfYZun3DqvfHh3DqGHSSSrpXF28mbN5uAZMogc9cbhRX0/bbLugLmCnl6rVp/38958ANazEYUZSWBUtb3xPALYoYhyp9XcrgmUAkpbSD1/pTGN8AjqhAB0tnpZXMJM5pw3bAx/poFxIateyYB3qqI1o/EbW966FAdzkHGJ6sENa0374B18YRkJpP5f/ul/V01wwgvgmJDny7wegILM6wAZk1JIfeFBgzn8qAEKYA7BOR+CLsMpHO6syyaN6G/gogpMy0K9YJ8/aog6xLidnKQdwer8gkmHXgTe677o5N2kAxWTOBxrYS9bANlBet9gZHvMAdX290AOmZ3ABI8tPawAj2eCz9GxNCE7CdA7usfBwCOC8hoiMZH/PrUYMwzJ0EN/G3Uf76uoAIoyGsx/UGP3krfHCfnqMMwo+FPEoDgxG4+BkutXoekltiZDW+WVg7qSj5vH0GhFXt5NSiKrdM7su6vhVI1pbfhH6NMg1rJ/qb0IAKOaetHwA2Qig9bt0iFn3540TsiyLDheqm1Cxf6RRETupXqb16S0U4W2lufqQME491lES5jw09tMsl9eGnJrWmmNdpy5EvzQmbHXfKjHqWGx2s0+uO5zlxVXlI6tpnFi80NmGfGVCOKCSnoi9bVPtb7s1w3r/cA47SyD8KJRhB2yjQo08hskIKynSinfyf+2WL9hGsG7+wkrN2MfZpZA593N2DbhyYdXGQ/lv/cIUwH8fhJKY74tdvQ5txEhjPE0e8wKjb6t4m+1o9b5RaaxxkmvrlwhWPKEJRNS+JMDjUgjRU63AagkWNMQhwJLcQjypo+AkoUlItR+oZ9Qu0dDbhY3XF+SFhdbnwYuHwUyI5p9nDLjQAb+nAUoZ3apDxzmph+ECTiFppVnF4j6McDgkKmr9eTf+2l8VZGytHo/tQTOlVC74UpjayiiwQrFSN0Arw9EaRPDRDc8KR091lrEFCUjnxmkWE58d5QOHBua/kgWCqgBrMmD3/snUWkOmSPTjSWIGvk6fA22yh+0BSUrydy4iZXrchKBPgKg8Tst8cKJV4bPPl7cXaMDRwSGnRGZwcR3gTwY+8hQtz2e2IlhPUJXtKPPCgwi5+WsF/uhfN7fg3xJOgN97PQ3eT0kUehd/ABwso+8P4p19ABCdc6BzVpDjcvkbyBAb0rkIAZ4cZ5fA/8fQNDkkh36obYt0OU1FaBNjnmoB1vJTfiiqF9t1ysZ3AYa7hpYZskSlG9Hkk2GGANXTKElF3uavsp3v+VdxB4gEOcXv6sXkYlih8LmyDdxTOtBt2BZKQFZev/LImm1XH14J99PWvUR0dwMS69uBsoRZQnKjrwdBe1W5fbFE9O27CMAB2g7udhfcNw7k0OKZK+47Jfi3xqlYfRNnpojmK5VtbksbdC9K4LEI7QVAexzPGVff9p6k0nny4eLZYZSpCrR8FRyDJG+I8HhQPRS5U83T87sfJtJoIcrGEckjzqGcTdTsxZr4+J0W5m8+Ah1VIYMLS1F6+oUVXiHrOeXNzJ4HG0y5UZKHFUXztZQVlgkKSnGqQAAAAAAAAAAAAA";
        // Enviar evento para o servidor
        await client.SendEvent("imagemessage", "nicolas$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");

        // Aguardar por mensagens recebidas
        var receiveTask = client.Receive();

        // Aguardar o usuário pressionar uma tecla para desconectar
        Console.WriteLine("Pressione qualquer tecla para desconectar.");
        Console.ReadKey();

        // Desconectar do servidor
        await client.Disconnect();

        // Aguardar a conclusão da tarefa de recebimento (para garantir limpeza correta)
        await receiveTask;
    }
}
