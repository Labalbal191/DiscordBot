using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using System.Threading;
using Discord;
using System.Diagnostics;
using Discord.Audio;

namespace DiscordBot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        [Command("Game", RunMode = RunMode.Async)]
        public async Task LetsPlay()
        {
            var random = new Random();
            var fileName = "Games.txt";
            var games = File.ReadAllLines(fileName);

            var randomNumber = random.Next(0, games.Count());

            await ReplyAsync("Let's play " + games[randomNumber]);
            Thread.Sleep(5000);
            await ReplyAsync("I'm bored, I'm going back to League");
        }

        [Command("Photo", RunMode = RunMode.Async)]
        public async Task Photo()
        {
            var random = new Random();
            var photos = Directory.GetFiles("photos");
            var randomNumber = random.Next(0, photos.Count());

            await Context.Channel.SendFileAsync(photos[randomNumber], "Me :)");
        }

        //Need to be fixed
        [Command("Hello", RunMode = RunMode.Async)]
        public async Task Hello()
        {
            await JoinChannel();
            await SendAsync("Hello.mp3");
        }


        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;

            if (channel == null)
            {
                await Context.Message.Channel.SendMessageAsync("Join to any channel first"); return;
            }

            await channel.ConnectAsync();
        }
        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        private async Task SendAsync(string path, IAudioClient client = null)
        {
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }

    }
}