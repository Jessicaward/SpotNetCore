![Twitter Follow](https://img.shields.io/twitter/follow/iamjessicaward?style=social)
![GitHub issues](https://img.shields.io/github/issues/jessicaward/spotnetcore)
![Lines of code](https://img.shields.io/tokei/lines/github/jessicaward/spotnetcore)
![Made By Fusion](https://img.shields.io/badge/made%20by-fusion.co.im-orange)

# SpotNetCore
Command-line Spotify controller written in C# using .NET 5.

# Contributions
| Contribution     | Profile     |
| :------------- | :----------: |
|  Owner | ![Jessica Ward](https://github.com/Jessicaward)   |
| Contributor   | ![srth21](https://github.com/srth21) |

## Commands
### Play
This command will play the current track.
```
play
```

### Pause
This command will pause the current track.
```
pause
```

### Current
This command will display the current track.
```
current
```

### Next
This command will skip the current track, playing the next track in the queue. It will then print information on the current track.
```
next
```

### Previous
This command will play the previous track. It will then print information on the current track.
*NOTE: This will not skip to the start of the current track, regardless of how far through the track you are. Use `restart` for this purpose.*
```
previous
```

### Restart
This command starts the current track from the beginning. It will then print information on the current track.
```
restart
```

### Shuffle
This command toggles shuffle on and off for the user's current player.
```
shuffle
```
Use the following *optional* parameters to specifically set a shuffle state:
#### Turn Shuffle On
```
shuffle on
```
```
shuffle true
```

#### Turn Shuffle Off
```
shuffle off
```
```
shuffle false
```
### Queue
The queue command allows the user to queue tracks, albums and artists.
Use the following *required* parameters and queries to queue a specific entity (track, playlist, album or artist).
#### Queue a track
```
queue --track everything in its right place
```
#### Queue an album
```
queue --album bonito generation
```

#### Queue a playlist
```
queue --playlist cozy autumn vibes
```

#### Queue an artist
When queueing an artist, the user has three options:
1. Queue the artist's entire discography.
```
queue --artist system of a down --discography
queue --artist system of a down --d
```

2. Queue the artist's "popular" tracks.
```
queue --artist wolf alice --popular
queue --artist wolf alice --p
```

3. Queue the artist's "This Is x" playlist.
```
queue --artist kendrick lamar --essential
queue --artist kendrick lamar --e
```

### Help
This command will display a list of supported SpotNetCore commands.
```
help
```

### Exit
This command will close the application.
```
exit
```

```
close
```

```
quit
```
