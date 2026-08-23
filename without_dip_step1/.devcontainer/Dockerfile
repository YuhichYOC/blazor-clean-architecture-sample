FROM debian:13-slim

ARG DEBIAN_FRONTEND=noninteractive

# 依存関係 ( curl, git など )
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates curl git \
    && rm -rf /var/lib/apt/lists/*

# Microsoft のパッケージ署名 / リポジトリ追加 ( .NET SDK インストール用 )
RUN curl -fsSL https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb -o packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm -f packages-microsoft-prod.deb

# .NET SDK インストール
# ( LTSを固定したい場合はバージョンを指定 )
RUN apt-get update && apt-get install -y --no-install-recommends \
    dotnet-sdk-8.0 \
    && rm -rf /var/lib/apt/lists/*

# 作業用ユーザー (任意だが VSCode の利便性のため推奨 )
# UID / GID は必要に応じて変更
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=1000

RUN groupadd --gid ${USER_GID} ${USERNAME} \
    && useradd -m --uid ${USER_UID} --gid ${USER_GID} -s /bin/bash ${USERNAME}

USER ${USERNAME}
WORKDIR /home/${USERNAME}

# 追加ツール ( 必要なら )
# RUN dotnet --info

CMD ["bash"]
