import streamlit as st
import requests

st.title("Assistant")

# Initialize the chat messages history
if "messages" not in st.session_state.keys():
    st.session_state.messages = [
        {"role": "assistant", "content": "How can I help?"}
    ]

# Prompt for user input and save
if prompt := st.chat_input():
    st.session_state.messages.append({"role": "user", "content": prompt})

# Display the prior chat messages
for message in st.session_state.messages:
    with st.chat_message(message["role"]):
        st.write(message["content"])


if st.session_state.messages[-1]["role"] != "assistant":
 
    with st.chat_message("assistant"):
        with st.spinner("Thinking..."):
            # Make an API call
            api_url = "https://localhost:7197/chat"  
            prompt = st.session_state.messages[-1]["content"]
            response = requests.post(api_url, data=prompt , verify=False).json()
            response = response.replace("," , "")
            response = response.replace("Bob:", "").replace("User:", "")

            # Display the API response
            st.write(response)  

    # Add the response to the chat history
    message = {"role": "assistant", "content": response}
    st.session_state.messages.append(message)
