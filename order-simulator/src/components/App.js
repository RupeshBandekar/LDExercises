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
      {this.props.portfolio.map(portfolioData => <PortfolioData key={portfolioData.id} {...portfolioData}/>)}
    </div>
    
    );
  };
}


class SendOrder extends React.Component{
  state = {txnDate: "", txnType: "0", asset: "0", quantity: "", price: "", txnStatus: "", failureReason: ""}
  //{asset: '0', quantity: '', price: ''};
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
            <select value={this.state.txnType} onChange={event => this.setState({txnType: event.target.value})}>
              <option value="0">Action</option>
              <option value="B">Buy</option>
              <option value="S">Sell</option>
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

class TransactionData extends React.Component{
  render(){
    const txnHistoryData = this.props;
    return(
      <div>        
        <div className="transaction-column">
        {txnHistoryData.txnDate}
        </div>
        <div className="transaction-column">
          {txnHistoryData.txnType}
        </div>
        <div className="transaction-column">
          {txnHistoryData.asset}
        </div>
        <div className="transaction-column">
          {txnHistoryData.quantity}
        </div>
        <div className="transaction-column">
          {txnHistoryData.price}
        </div>        
        <div className="transaction-column">
          {txnHistoryData.txnStatus}
        </div>
        <div className="transaction-column">
          {txnHistoryData.failureReason}
        </div>
      </div>
    );
  };
}

class TransactionHistory extends React.Component{
  render(){
    return(
      <div>
        <div>
          Transaction History:
        </div>
        <div className="transaction-column">
          Txn Time
        </div>
        <div className="transaction-column">
          Txn Type
        </div>
        <div className="transaction-column">
          Asset
        </div>
        <div className="transaction-column">
          Quantity
        </div>        
        <div className="transaction-column">
          Txn Amount
        </div>
        <div className="transaction-column">
          Txn Status
        </div>
        <div className="transaction-column">
          Remarks
        </div>
        {this.props.txnHistory.map(txnData => <TransactionData key={txnData.id} {...txnData}/>)}
      </div>
    );
  };
}

class App extends React.Component {

  state = {
    portfolio: [
      {quantity: 100, asset: "GOOGL", price: 250.00},
      {quantity: 250, asset: "MO", price: 412.00},
      {quantity: 325, asset: "AMZN", price: 645.25},
    ],

    txnHistory: [
      {txnDate: "6/3/2019 2:19:15 PM", txnType: "B", asset: "GOOGL", quantity: 500, price: 105.25, txnStatus: "S", failureReason: ""},
      {txnDate: "6/3/2019 2:20:55 PM", txnType: "B", asset: "GOOGL", quantity: 500, price: 105.25, txnStatus: "S", failureReason: ""},
      {txnDate: "6/3/2019 2:22:30 PM", txnType: "S", asset: "GOOGL", quantity: 1005, price: 105.25, txnStatus: "F", failureReason: "Quantity for sell exceeds the quantity available"},
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
    }    // console.log("inside App");
  };

  addNewTxnHistory = (txnData) => {
    var today = new Date();
    var date = today.getDate() + '-' + (today.getMonth()+1) + '-' + today.getFullYear();
    var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
    var dateTime = date + ' ' + time;

    txnData.txnDate = dateTime;

    this.setState(prevTxn => ({
      txnHistory: [...prevTxn.txnHistory, txnData],
    }));
  };

  
  validateOrder = (orderData) => {
    //https://stackoverflow.com/questions/29537299/react-how-do-i-update-state-item1-on-setstate-with-jsfiddle
    console.log("inside validate order");
    console.log(orderData.txnType);
    if(orderData.txnType === "S")
    {
      const portfolioPosition = this.state.portfolio.filter(x => x.asset === orderData.asset);
      if(portfolioPosition.length > 0)
      {
        //if asset is present in portfolio
        if(portfolioPosition[0].quantity < orderData.quantity)
        {
          orderData.txnStatus = "F";
          orderData.failureReason = "Quantity for sell exceeds the portfolio quantity";
        }
      }
      else
      {
        //asset is not present in portfolio
        orderData.txnStatus = "F";
        orderData.failureReason = "Asset for sell does not exist in portfolio";
      }
    }

    this.setState()

    this.addNewTxnHistory(orderData);
  };

  render(){
    return (
      <div>        
        <Portfolio portfolio={this.state.portfolio}/>
        <SendOrder onSubmit={this.validateOrder} />
        <TransactionHistory txnHistory={this.state.txnHistory}/>
      </div>
    );
  }
}

export default App;
