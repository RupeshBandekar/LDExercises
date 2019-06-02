import React from 'react';
import '../styles/App.css';

class PortfolioData extends React.Component{  
  render(){
    const portfolioData = this.props;
    return(
      <div>
        <div className="portfolio-column">
          {portfolioData.asset}
        </div>
        <div className="portfolio-column">
          {portfolioData.quantity}
        </div>
        <div className="portfolio-column">
          {portfolioData.price}
        </div>
        <div className="portfolio-column">
          Buy/Sell
        </div>
      </div>
    );
  }
}

class Portfolio extends React.Component{  
  render(){
    return(
    <div>
      <div>
      Portfolio:
      </div>
      <div className="portfolio-column">
        Security
      </div>
      <div className="portfolio-column">
        Quantity
      </div>
      <div className="portfolio-column">
        Buy Price
      </div>
      <div className="portfolio-column">
        Action
      </div>
      {this.props.portfolio.map(portfolioData => <PortfolioData key={portfolioData.id} {...portfolioData}/>)}
    </div>
    
    );
  };
}


class SendOrder extends React.Component{
  state = {action:'0', quantity: '', price: '', asset: '0'};
  handleSendOrder = event => {
    event.preventDefault();
    this.props.onSubmit(this.state);
    //this.setState({action:'0', quantity: '', price: '', asset: '0'});
  };
  render(){
    return(
      <form onSubmit={this.handleSendOrder}>
        <div>
          Send Order:
        </div>
        <div>
          <div className="sendorder-column">
            <select value={this.state.action} onChange={event => this.setState({action: event.target.value})}>
              <option value="0">Action</option>
              <option value="Buy">Buy</option>
              <option value="Sell">Sell</option>
            </select>          
          </div>
          <div className="sendorder-column">
          <input type="text" placeholder="Quantity" value={this.state.quantity} onChange={event => this.setState({quantity: event.target.value})}></input>
          </div>
          <div className="sendorder-column">
            Asset:
            <select value={this.state.asset} onChange={event => this.setState({asset: event.target.value})}>
              <option value="0">Select</option>
              <option value="GOOGL" title="	Alphabet Inc Class A">GOOGL</option>
              <option value="MO" title="Altria Group Inc">MO</option>
              <option value="AMZN" title="Amazon.com Inc.">AMZN</option>
              <option value="AAPL" title="Apple Inc.">AAPL</option>
              <option value="BLK" title="BlackRock">BLK</option>
              <option value="BA" title="Boeing Company">BA</option>
              <option value="CSCO" title="Cisco Systems">CSCO</option>
              <option value="CTXS" title="Citrix Systems">CTXS</option>
              <option value="KO" title="Coca-Cola Company">KO</option>
              <option value="CTSH" title="Cognizant Technology Solutions">CTSH</option>
            </select>          
          </div>
          <div className="sendorder-column">
            <input type="text" placeholder="Price" value={this.state.price} onChange={event => this.setState({price: event.target.value})}></input>
          </div>
        </div>
        <div>
          <button>Send Order</button>
        </div>
      </form>
    );
  };
}

class App extends React.Component {

  state = {
    portfolio: [
      {action: "Buy", quantity: "100", asset: "GOOGL", price: "250.00"},
      {action: "Buy", quantity: "250", asset: "MO", price: "412.00"},
      {action: "Buy", quantity: "325", asset: "AMZN", price: "645.25"},
    ],
  };

  addNewPortfolioData = (portfolioData) => {

  const item = this.state.portfolio.filter(x => x.asset === portfolioData.asset);
  if(item.length > 0)
  {
    console.log(item);
  }
  else {
    this.setState(prevPortfolio => ({
      portfolio: [...prevPortfolio.portfolio, portfolioData],
    }));
  }

    

    // console.log("inside App");
  };

  render(){
    return (
      <div>
        <Portfolio portfolio={this.state.portfolio}/>
        <SendOrder onSubmit={this.addNewPortfolioData} />
      </div>
    );
  }
}

export default App;
