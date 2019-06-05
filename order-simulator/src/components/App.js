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
          {portfolioData.quantity * portfolioData.price}
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
        Current Price
      </div>
      {this.props.portfolio.map(portfolioData => <PortfolioData key={portfolioData.id} {...portfolioData}/>)}
    </div>
    
    );
  };
}


class SendOrder extends React.Component{
  state = {txnDate: "", txnType: "0", asset: "0", quantity: "", price: "", txnStatus: "", failureReason: "", errorMessage: ""};
  handleSendOrder = event => {
    event.preventDefault();
    this.props.onSubmit(this.state);
    this.setState({txnDate: "", txnType: "0", asset: "0", quantity: "", price: "", txnStatus: "", failureReason: ""});
  };

  acceptValidInput = (event) => {
    switch(event.target.id)
    {
      case "txtQuantity": 
        {
          const re = /^[1-9]+\d*$/;
          if (event.target.value === '' || re.test(event.target.value)) {
            this.setState({quantity: event.target.value});
          }
        }
        break;
      case "txtPrice":
        {
          const re = /^[1-9]+\d*\.?\d*?$/;
          if (event.target.value === '' || re.test(event.target.value)) {
            this.setState({price: event.target.value});
          }   
        }
        break;
      default:
    }
  }

  render(){
    return(
      <form onSubmit={this.handleSendOrder}>
        <div>
          Send Order:
        </div>
        <div>
          <div className="sendorder-column">
            <select id="ddlAction" value={this.state.txnType} 
            onChange={event => {this.setState({txnType: event.target.value}); this.props.onChange();}} required>
              <option value="">Action</option>
              <option value="B">Buy</option>
              <option value="S">Sell</option>
            </select>          
          </div>
          <div className="sendorder-column">
          <input id="txtQuantity" type="text" value={this.state.quantity} placeholder="Quantity" 
          onChange={event => {this.acceptValidInput(event); this.props.onChange();}} required></input>
          </div>
          <div className="sendorder-column">
            Asset:
            <select id="ddlAsset" value={this.state.asset} onChange={event => {this.setState({asset: event.target.value}); this.props.onChange();}} required>
              <option value="">Select</option>
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
            <input id="txtPrice" type="text" placeholder="Price" value={this.state.price} 
            onChange={event => {this.acceptValidInput(event); this.props.onChange();}} required></input>
          </div>
        </div>
        <div>
          <button>Send Order</button>          
        </div>
        <div>{this.state.errorMessage}</div>
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

class ValidationSummary extends React.Component {  
  render(){
    return(
      <div>{this.props.errorMessage}</div>
    );
  };
}

class App extends React.Component {

  state = {
    portfolio: [
      {asset: "GOOGL", quantity: 100, price: 200.00},
      {asset: "MO", quantity: 200, price: 150.00},
      {asset: "AMZN", quantity: 250, price: 250},
    ],

    txnHistory: [
      {txnDate: "6/3/2019 2:19:15 PM", txnType: "B", asset: "GOOGL", quantity: 500, price: 105.25, txnStatus: "S", failureReason: ""},
      {txnDate: "6/3/2019 2:20:55 PM", txnType: "B", asset: "GOOGL", quantity: 500, price: 105.25, txnStatus: "S", failureReason: ""},
      {txnDate: "6/3/2019 2:22:30 PM", txnType: "S", asset: "GOOGL", quantity: 1005, price: 105.25, txnStatus: "F", failureReason: "Quantity for sell exceeds the quantity available"},
    ],

    errorMessage: '',
  };

  updatePortfolio = (orderData) => {
    
    const index = this.state.portfolio.findIndex(x => x.asset === orderData.asset);
    if(index > -1)
    {      
      const copyPortfolio = [...this.state.portfolio];
      let copyPortfolioAsset = copyPortfolio[index];
      let portfolioQuantity = parseInt(copyPortfolioAsset.quantity);
      let orderQuantity = parseInt(orderData.quantity);
      if(orderData.txnType === "S")
      {
        if(portfolioQuantity - orderQuantity === 0)
        {
          //removing asset from portfolio
          const splicedPortfolio = this.state.portfolio.splice(index, 1);
          this.setState({splicedPortfolio});
        }
        else {
          portfolioQuantity = portfolioQuantity - orderQuantity;
        }
      }
      else if(orderData.txnType === "B")
      {
        portfolioQuantity = portfolioQuantity + orderQuantity;
      }
      
      if(parseInt(copyPortfolioAsset.quantity) !== portfolioQuantity)
      {        
        copyPortfolioAsset.quantity = portfolioQuantity;
        copyPortfolio[index] = copyPortfolioAsset;
        this.setState({copyPortfolio});
      }
    }
    else {
      this.setState(prevPortfolio => ({
        portfolio: [...prevPortfolio.portfolio, orderData],
      }));
    }
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

  validateQuantity = (orderQuantity) => {    
    const re = /^[1-9]+\d*$/;
    if (!(orderQuantity === '' || re.test(orderQuantity))) {
      return false;
    }
    else return true;
  }

  validatePrice = (orderPrice) => {
    const re = /^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$/;
    if (re.test(orderPrice)) {
      return true;
    } 
    else return false;       
  }

  resetErrorMessage = () => {
    this.setState({errorMessage: ''});
  }

  validateOrder = (orderData) => {
    //https://stackoverflow.com/questions/29537299/react-how-do-i-update-state-item1-on-setstate-with-jsfiddle    
    
    if(!(this.validatePrice(orderData.price))) {
      this.setState({errorMessage: 'Invalid price'});
    }
    else {  
      const portfolioPosition = this.state.portfolio.filter(x => x.asset === orderData.asset);
      orderData.txnStatus = "S";

      if(portfolioPosition.length > 0)
      {
        orderData.price = portfolioPosition[0].price;
      }

      if(orderData.txnType === "S")
      {      
        if(portfolioPosition.length > 0)
        {
          //if asset is present in portfolio
          if(portfolioPosition[0].quantity < orderData.quantity)
          {
            orderData.txnStatus = "F";
            orderData.failureReason = "Quantity for sell exceeds the portfolio quantity";
            this.setState({errorMessage: 'Quantity for sell exceeds the portfolio quantity'});
          }
        }
        else
        {
          //asset is not present in portfolio
          orderData.txnStatus = "F";
          orderData.failureReason = "Asset for sell does not exist in portfolio";
          this.setState({errorMessage: 'Asset for sell does not exist in portfolio'});
        }
      }
      
      this.addNewTxnHistory(orderData);

      if(orderData.txnStatus === "S") {
        console.log(portfolioPosition);
        this.updatePortfolio(orderData);
      }
    }
  };

  render(){
    return (
      <div>        
        <Portfolio portfolio={this.state.portfolio}/>
        <SendOrder onSubmit={this.validateOrder} onChange={this.resetErrorMessage}/>
        <ValidationSummary errorMessage={this.state.errorMessage}/>
        <TransactionHistory txnHistory={this.state.txnHistory}/>
      </div>
    );
  }
}

export default App;
