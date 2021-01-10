![Twitter Follow](https://img.shields.io/twitter/follow/iamjessicaward?style=social)
![GitHub issues](https://img.shields.io/github/issues/jessicaward/spotnetcore)
![Lines of code](https://img.shields.io/tokei/lines/github/jessicaward/spotnetcore)
![Made By Fusion](https://img.shields.io/badge/made%20by-fusion.im-orange)

# SpotNetCore
Command-line Spotify client written in .NET Core

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
This command will skip the current track, playing the next track in the queue. It will then print the current track.
```
next
```

### Previous
This command will play the previous track. It will then print the current track.
*NOTE: This will not skip to the start of the current track, regardless of how far through the track you are. Use `restart` for this purpose.*
```
previous
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
