from flask import Flask, request, jsonify
import requests
import json

app = Flask(__name__)

API_KEY = "AIzaSyDBpxn8QjraUqZFC-93yqk8JcYFsA_8vCo"
GEMINI_URL = f"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={API_KEY}"

def postprocess_text(text):
            text.strip().strip('"').strip("'")
            return text

@app.route("/object_spawned", methods=["POST"])
def object_spawned():
    data = request.json
    environment_state = data.get("environment_state", "Neutral")
    #circumstances = data.get("circumstances", "You are in a forest.")
    god_action = data.get("god_action", "A mysterious object appears!")
    char_profession = data.get("char_profession", "Character profession.")

    prompt = (
        "You are a character in a survival game where a god controls your environment.\n"
        f"You must respond in-character to god's actions as a {char_profession} (no narration or action description).\n"
        #"You must respond in-character to god's actions (no narration or action description).\n"
        f"Current state of the world: {environment_state}\n"
        f"God's action: {god_action}\n"
        "What do you say?"
    )

    headers = { "Content-Type": "application/json" }
    payload = {
        "contents": [
            {
                "parts": [
                    {"text": prompt}
                ]
            }
        ]
    }

    gemini_response = requests.post(GEMINI_URL, headers=headers, data=json.dumps(payload))
    if gemini_response.status_code == 200:
        result = gemini_response.json()
        try:
            response_text = result["candidates"][0]["content"]["parts"][0]["text"].strip('"').strip("'")
            response_text = postprocess_text(response_text)
            return response_text[:-2]
            #return jsonify({"response": response_text})
        except (KeyError, IndexError):
            return jsonify({"error": "Failed to parse response text."}), 500
    else:
        return jsonify({"error": gemini_response.text}), gemini_response.status_code

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
