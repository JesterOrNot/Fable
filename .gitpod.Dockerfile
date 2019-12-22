FROM gitpod/workspace-full
USER gitpod
# Install F#
RUN sudo apt-get update \
	&& sudo apt-get install -y \
		fsharp
# Install .NET
RUN wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
	&& sudo dpkg -i packages-microsoft-prod.deb \
	&& sudo apt-get update \
	&& sudo apt-get update \
	&& sudo apt-get install -y \
		apt-transport-https \
		dotnet-sdk-3.0
