FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100

# Register Microsoft key and feed
RUN wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb

# Install DotNetCore 2.1 Runtime-Only for SonarScanner
RUN apt-get update -y \
    && apt-get install apt-transport-https -y \
    && apt-get update -y \
    && apt-get install aspnetcore-runtime-2.2 -y

# Install Java
RUN sudo apt-get update
RUN sudo apt-get install -t openjdk-8-jdk

ENV JAVA_HOME /usr/lib/jvm/java-8-openjdk-amd64