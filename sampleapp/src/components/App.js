import React, {useState} from 'react';
import Game from "./Game";
import '../styles/App.css';

const StarMatch = () =>{
  const [gameId, setGameId] = useState(1);
  return(
    <Game key={gameId} startNewGame={() => setGameId(gameId + 1)}/>
  )
};

function App() {
  return (
    <StarMatch />
  );
}

export default App;
