import streamlit as st
from llama_cpp import Llama

llm = Llama(model_path="C:/Users/kerem/Desktop/llmhotel/Model/llama-2-7b-guanaco-qlora.Q4_K_M.gguf")
st.title("Assistant")


# Initialize the chat messages history
if "messages" not in st.session_state.keys():
    st.session_state.messages = [
        {"role": "assistant", "content": "How can I help?"}
    ]
    
    

# Prompt for user input and save
if prompt := st.chat_input():
    st.session_state.messages.append({"role": "user", "content": prompt})
    
    

# display the prior chat messages
for message in st.session_state.messages:
    with st.chat_message(message["role"]):
        st.write(message["content"])    
        
        
        
# If last message is not from assistant, we need to generate a new response
if st.session_state.messages[-1]["role"] != "assistant":
    # Call LLM
    with st.chat_message("assistant"):
        with st.spinner("Thinking..."):
        
            user_messages = [message["content"] for message in st.session_state.messages if message["role"] == "user"]
            
            user_input = "\n".join(user_messages)
            
            response = llm(user_input)
        
            st.write(response.choices[0])

    message = {"role": "assistant", "content": response}
    st.session_state.messages.append(message)