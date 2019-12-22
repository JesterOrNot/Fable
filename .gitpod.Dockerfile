FROM gitpod/workspace-full
USER gitpod

# Install .NET
RUN wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
	&& sudo dpkg -i packages-microsoft-prod.deb \
	&& sudo apt-get update \
&& sudo apt-get install -y \
        fsharp \
		apt-transport-https \
		dotnet-sdk-3.1
