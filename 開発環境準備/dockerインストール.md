# Ubuntu
- https://docs.docker.com/engine/install/ubuntu/
```bash
$ sudo apt update
$ sudo apt install ca-certificates curl
$ sudo install -m 0755 -d /etc/apt/keyrings
$ sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
$ sudo chmod a+r /etc/apt/keyrings/docker.asc
# Add the repository to Apt sources:
$ sudo tee /etc/apt/sources.list.d/docker.sources <<EOF
Types: deb
URIs: https://download.docker.com/linux/ubuntu
Suites: $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}")
Components: stable
Architectures: $(dpkg --print-architecture)
Signed-By: /etc/apt/keyrings/docker.asc
EOF
$ sudo apt update
$ sudo apt install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

# Debian
- https://docs.docker.com/engine/install/debian/
```bash
$ sudo apt update
$ sudo apt install ca-certificates curl
$ sudo install -m 0755 -d /etc/apt/keyrings
$ sudo curl -fsSL https://download.docker.com/linux/debian/gpg -o /etc/apt/keyrings/docker.asc
$ sudo chmod a+r /etc/apt/keyrings/docker.asc
# Add the repository to Apt sources:
$ sudo tee /etc/apt/sources.list.d/docker.sources <<EOF
Types: deb
URIs: https://download.docker.com/linux/debian
Suites: $(. /etc/os-release && echo "$VERSION_CODENAME")
Components: stable
Architectures: $(dpkg --print-architecture)
Signed-By: /etc/apt/keyrings/docker.asc
EOF
$ sudo apt update
$ sudo apt install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

# apt install ca-certificates
- インストール済みならスキップしていい
```bash
$ apt list ca-certificates
Listing... Done
ca-certificates/noble-updates,noble-security 20260601~24.04.1 all [upgradable # 末尾に [installed] や [upgradable] がついているならインストール済み
```

# apt install curl
- インストール済みならスキップしていい
```bash
$ apt list curl
Listing... Done
curl/noble-updates,noble-security 8.5.0-2ubuntu10.11 amd64 [upgradable # 末尾に [installed] や [upgradable] がついているならインストール済み
```

# install -m 0755 -d /etc/apt/keyrings
- /etc/apt/keyrings が既に存在する, パーミッションが drwxr-xr-x ならスキップしていい
```bash
$ ls -l /etc/apt/
total 28
drwxr-xr-x 2 root root 4096 Apr  1  2024 keyrings
```
