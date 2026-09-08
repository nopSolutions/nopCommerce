use std::io::{BufRead, BufReader, Write};
use std::process::{Command, Stdio};

#[test]
fn spawn_reads_json_line_and_returns_bankers_midpoint() {
    let bin = env!("CARGO_BIN_EXE_nop-round");
    let mut child = Command::new(bin)
        .stdin(Stdio::piped())
        .stdout(Stdio::piped())
        .stderr(Stdio::inherit())
        .spawn()
        .expect("spawn nop-round sidecar");

    {
        let mut stdin = child.stdin.take().expect("sidecar stdin");
        writeln!(
            stdin,
            r#"{{"value":"1.225","roundingType":"Rounding001"}}"#
        )
        .expect("write request");
        stdin.flush().expect("flush request");
        drop(stdin);
    }

    let stdout = child.stdout.take().expect("sidecar stdout");
    let mut lines = BufReader::new(stdout).lines();
    let line = lines.next().expect("sidecar response").expect("read line");
    let status = child.wait().expect("wait sidecar");

    assert!(status.success(), "sidecar exit {status}");
    let v: serde_json::Value = serde_json::from_str(&line).expect("json response");
    assert_eq!(v["rounded"], "1.22", "sidecar body: {line}");
}
