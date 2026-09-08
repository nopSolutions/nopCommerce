use std::io::{self, BufRead, Write};

use serde::Deserialize;
use serde::Serialize;

use nop_round::round_str;

#[derive(Deserialize)]
struct RoundRequest {
    value: String,
    #[serde(rename = "roundingType")]
    rounding_type: String,
}

#[derive(Serialize)]
struct RoundResponse {
    rounded: String,
}

#[derive(Serialize)]
struct ErrorResponse {
    error: String,
}

fn handle_line(line: &str) -> String {
    match serde_json::from_str::<RoundRequest>(line) {
        Ok(req) => match round_str(&req.value, &req.rounding_type) {
            Ok(rounded) => serde_json::to_string(&RoundResponse { rounded }).unwrap(),
            Err(error) => serde_json::to_string(&ErrorResponse { error }).unwrap(),
        },
        Err(e) => serde_json::to_string(&ErrorResponse {
            error: e.to_string(),
        })
        .unwrap(),
    }
}

fn main() {
    let stdin = io::stdin();
    let mut stdout = io::stdout();
    for line in stdin.lock().lines() {
        let line = line.expect("stdin");
        if line.trim().is_empty() {
            continue;
        }
        writeln!(stdout, "{}", handle_line(&line)).expect("stdout");
        stdout.flush().expect("flush");
    }
}
