import React from 'react';
import '../styles/App.css';



class Portfolio extends React.Component{
  render(){
    return(
    <div>
      <div className="portfolio-column">
        Security
      </div>
      <div className="portfolio-column">
        Quantity
      </div>
      <div className="portfolio-column">
        But Price
      </div>
      <div className="portfolio-column">
        Action
      </div>
    </div>
    
    );
  };
}

function App() {
  return (
    <Portfolio />
  );
}

export default App;
