#:property PublishAot=false
using System.Text.Json;
using System.Text.Json.Serialization;

// Create unions and closed typed hierarchies for Fees
// A fee can be either no fee, a percentage or a fixed value