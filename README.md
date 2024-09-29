# UdpStressBench
ランダムなバイト列をUDPで特定のアドレスに送信する検証用アプリケーション

## オプション
カッコ内はデフォルト値

- `-address <address>` : 宛先アドレス (127.0.0.1)
- `-port <port>` : 宛先ポート (8001)
- `-size <size>` : パケットサイズ (1024)
- `-rate <rate>` : パケットレート (1)

## 例
デフォルト設定を使用する場合:

`UdpStressBench.exe`

カスタムアドレス、ポート、サイズ、レートを指定する場合:

`UdpStressBench.exe -address 192.168.0.1 -port 8080 -size 1024 -rate 500`

## ヘルプ表示
以下のいずれかの引数を使用すると、ヘルプメッセージが表示されます
- `-help`
- `-h`
- `--help`