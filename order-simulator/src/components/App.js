import React from 'react';
import '../styles/App.css';

class PortfolioData extends React.Component{  
  render(){
    const portfolioData = this.props;
    return(
      <div className="rTableRow">
        <div className="rTableCell">
          {portfolioData.asset}
        </div>
        <div className="rTableCell">
          {portfolioData.quantity}
        </div>
        <div className="rTableCell">
          {new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(portfolioData.price)}
        </div>
        <div className="rTableCell">
          {new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(portfolioData.quantity * portfolioData.price)}
        </div>
      </div>
    );
  }
}

const PortfolioValue = (props) => (
    <div className="value">      
      {new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(
        props.portfolioData.length > 0 ? props.portfolioData.map(x => x.quantity * x.price).reduce((a,b) => a + b) : 0)}
    </div>  
);

class Portfolio extends React.Component{
  render(){
    return(
    <div className="portfolio-container">
      <div className="portfolio">      
        <div className="text">
          Portfolio value :
        </div>
        <PortfolioValue portfolioData={this.props.portfolio} />
      </div> 
      <div className="rTable">
        <div className="rTableRow">
          <div className="rTableHead">
            Security
          </div>
          <div className="rTableHead">
            Quantity
          </div>
          <div className="rTableHead">
            Buy Price
          </div>
          <div className="rTableHead">
            Total Price
          </div>
        </div>
        {this.props.portfolio.map(portfolioData => <PortfolioData key={portfolioData.id} {...portfolioData}/>)}
      </div>
      
    </div>
    
    );
  };
}


class SendOrder extends React.Component{
  state = {txnDate: "", txnType: "0", asset: "0", quantity: "", price: "", txnStatus: "", failureReason: ""};
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
        <div className="sendOrder">
          <div className="text">
            Send Order
          </div>
        </div>
        <div className="sendOrder-container">
          <div className="sendOrder-panel">
            <div>
              <select id="ddlAction" value={this.state.txnType} 
              onChange={event => {this.setState({txnType: event.target.value}); this.props.onChange();}} required>
                <option value="">Action</option>
                <option value="B">Buy</option>
                <option value="S">Sell</option>
              </select>          
            </div>
            <div>
            <label>Quantity:</label>
            <input id="txtQuantity" type="text" value={this.state.quantity} placeholder="0" 
            onChange={event => {this.acceptValidInput(event); this.props.onChange();}} required></input>
            </div>
            <div>
              <label>Asset:</label>
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
            <div>
              <label>Price:</label>
              <input id="txtPrice" type="text" placeholder="0.00" value={this.state.price} 
              onChange={event => {this.acceptValidInput(event); this.props.onChange();}} required></input>
            </div>
          </div>
          <div className="btn">
            <button className="button">Send Order</button>
          </div>
        </div>
      </form>
    );
  };
}

class TransactionData extends React.Component{
  render(){
    const txnHistoryData = this.props;
    return(
      <div className="rTableRow">
        <div className="rTableCell">
          {new Intl.DateTimeFormat('en-US', {day:'2-digit',month:'2-digit',year:'numeric',hour:'2-digit',minute:'2-digit',second:'2-digit',hour12: false}).format(txnHistoryData.txnDate)}
          {/* {txnHistoryData.txnDate} */}
        </div>
        <div className="rTableCell">
          {txnHistoryData.txnType}
        </div>
        <div className="rTableCell">
          {txnHistoryData.asset}
        </div>
        <div className="rTableCell">
          {txnHistoryData.quantity}
        </div>
        <div className="rTableCell">
          {new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(txnHistoryData.price)}
        </div>
        <div className="rTableCell">
          {new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(txnHistoryData.quantity * txnHistoryData.price)}
        </div>        
        <div className={txnHistoryData.txnStatus === "Success" ? "rTableCellGreen" : "rTableCellRed"}>
          {txnHistoryData.txnStatus}
        </div>
        <div className="rTableCell">
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
        <div className="txnHistory">
          <div className="text">
            Transaction History
          </div>
        </div>
        <div className="test">
        <div className="rTable">
          <div className="rTableRow">
            <div className="rTableHead">
              Txn Time
            </div>
            <div className="rTableHead">
              Txn Type
            </div>
            <div className="rTableHead">
              Asset
            </div>
            <div className="rTableHead">
              Quantity
            </div>   
            <div className="rTableHead">
              Price
            </div>        
            <div className="rTableHead">
              Txn Amount
            </div>
            <div className="rTableHead">
              Txn Status
            </div>
            <div className="rTableHead">
              Remarks
            </div>
          </div>
          {this.props.txnHistory.map(txnData => <TransactionData key={txnData.id} {...txnData}/>)}
        </div>
        </div>
      </div>
    );
  };
}

class ValidationSummary extends React.Component {  
  render(){    
    return(      
      <div className={this.props.styleName}>{this.props.errorMessage}</div>
    );
  };
}

class App extends React.Component {

  state = {
    portfolio: [],
    txnHistory: [],
    errorMessage: '',
    // portfolio: [
    //   {asset: "GOOGL", quantity: 100, price: 200.00},
    //   {asset: "MO", quantity: 200, price: 150.00},
    //   {asset: "AMZN", quantity: 250, price: 250},
    // ],

    // txnHistory: [
    //   {txnDate: "6/3/2019 14:19:15", txnType: "B", asset: "GOOGL", quantity: 500, price: 105.25, txnStatus: "Success", failureReason: ""},
    //   {txnDate: "6/3/2019 14:20:55", txnType: "B", asset: "GOOGL", quantity: 500, price: 105.25, txnStatus: "Success", failureReason: ""},
    //   {txnDate: "6/3/2019 14:22:30", txnType: "S", asset: "GOOGL", quantity: 1005, price: 105.25, txnStatus: "Fail", failureReason: "Quantity for sell exceeds the quantity available"},      
    // ],

    
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
          console.log("zero");
          //removing asset from portfolio
          const splicedPortfolio = this.state.portfolio.splice(index, 1);
          //this.setState({splicedPortfolio});
          this.setState({splicedPortfolio})
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
        this.setState({portfolio: copyPortfolio});
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
    // var date = today.getDate() + '-' + (today.getMonth()+1) + '-' + today.getFullYear();
    // var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
    // var dateTime = date + ' ' + time;

    txnData.txnDate = today;

    // this.setState(prevTxn => ({
    //   txnHistory: [...prevTxn.txnHistory, txnData],
    // }));
    const prevtxnHistory = this.state.txnHistory.concat(txnData);
    this.setState({txnHistory: prevtxnHistory}); 
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
      this.setState({errorMessage: 'Alert: Invalid price'});
    }
    else {  
      const portfolioPosition = this.state.portfolio.filter(x => x.asset === orderData.asset);
      orderData.txnStatus = "Success";

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
            orderData.txnStatus = "Fail";
            orderData.failureReason = "Quantity for sell exceeds the portfolio quantity";
            this.setState({errorMessage: 'Alert: Quantity for sell exceeds the portfolio quantity'});
          }
        }
        else
        {
          //asset is not present in portfolio
          orderData.txnStatus = "Fail";
          orderData.failureReason = "Asset for sell does not exist in portfolio";
          this.setState({errorMessage: 'Alert: Asset for sell does not exist in portfolio'});
        }
      }
      
      this.addNewTxnHistory(orderData);

      if(orderData.txnStatus === "Success") {
        this.updatePortfolio(orderData);
        this.setState({errorMessage: 'Order placed successfully!'});
      }
    }
  };

  render(){
    let alertMessage;
    if(this.state.errorMessage !== "")
    {
      if(this.state.errorMessage.substring(0, 5) === "Alert") {        
        alertMessage = <ValidationSummary errorMessage={this.state.errorMessage} styleName={"alert"}/>
      }
      else {
        alertMessage = <ValidationSummary errorMessage={this.state.errorMessage} styleName={"alert-success"}/>
      }
    }
    return (
      <div>
        <div className="split left">
          <div className="aligned">
            <Portfolio portfolio={this.state.portfolio}/>        
            <SendOrder onSubmit={this.validateOrder} onChange={this.resetErrorMessage} validationMessage={this.state.errorMessage}/>
            {alertMessage}
          </div>
        </div>
        <div className="split aligned vl"></div>
        <div className="split right">
          <div className="aligned">
            <TransactionHistory txnHistory={this.state.txnHistory}/>
          </div>
        </div>
      </div>
    );
  }
}

export default App;
