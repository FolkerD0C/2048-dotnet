<h1 align="center">2048ish</h1>

<h4 align="center"> 
	2048ish
</h4> 

<hr>

<p align="center">
  <a href="#dart-about">About</a> &#xa0; | &#xa0; 
  <a href="#sparkles-features">Features</a> &#xa0; | &#xa0;
  <a href="#rocket-technologies">Technologies</a> &#xa0; | &#xa0;
  <a href="#white_check_mark-requirements">Requirements</a> &#xa0; | &#xa0;
  <a href="#checkered_flag-starting">Starting</a> &#xa0; | &#xa0;
  <a href="#memo-license">License</a> &#xa0; | &#xa0;
  <a href="https://github.com/FolkerD0C" target="_blank">Author</a>
</p>

<br>

## :dart: About ##

This project was (and still is) a learning project for me to learn to use neovim and git on the command line
and to improve my Linux knowledge. Up until commit d936c487 it was only developed in WSL2, using Ubuntu 22.04.6.

The game is simple: you need to have the tiles adding up to 2048.

Or not, because you can configure anything.

## :sparkles: Features ##

:heavy_check_mark: Configure game goal, maximum lives, maximum undos, grid height, grid width, accepted spawnables and the number of starter tiles;\
:heavy_check_mark: Saving/Loading;
:heavy_check_mark: A fully featured, layered backend and terminal frontend;

<img src="resources/2048ish.png" alt="Screenshot" title="Screenshot of the game">

## :white_check_mark: Requirements ##

Before starting :checkered_flag:, you need to have [Git](https://git-scm.com) and [dotnet](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed.

## :checkered_flag: Starting ##

```bash
# Clone this project
git clone https://github.com/FolkerD0C/2048ish

# Cd into 2048ish
cd 2048ish

# Build the terminal client
dotnet build ConsoleClient/ConsoleClient.App/ConsoleClient.App.csproj

```
<img src="resources/2048ish-gameplay.gif" alt="Gameplay">

## Future plans ##

- I plan to add the Apache Thrift API so I can have multiple clients in different languages

## :memo: License ##

This project is under license from the GNU General Public License. For more details, see the [LICENSE](LICENSE) file.


Made with :heart: by <a href="https://github.com/FolkerD0C" target="_blank">FolkerD0C</a>

&#xa0;

<a href="#top">Back to top</a>
