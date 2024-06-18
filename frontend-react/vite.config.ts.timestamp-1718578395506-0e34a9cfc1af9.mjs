// vite.config.ts
import { defineConfig } from "file:///C:/Users/mikae/Documents/GitHub/KafkaAuction/frontend-react/node_modules/vite/dist/node/index.js";
import react from "file:///C:/Users/mikae/Documents/GitHub/KafkaAuction/frontend-react/node_modules/@vitejs/plugin-react-swc/index.mjs";
import path from "path";
import { TanStackRouterVite } from "file:///C:/Users/mikae/Documents/GitHub/KafkaAuction/frontend-react/node_modules/@tanstack/router-vite-plugin/dist/esm/index.js";
var __vite_injected_original_dirname = "C:\\Users\\mikae\\Documents\\GitHub\\KafkaAuction\\frontend-react";
var isTest = process.env.NODE_ENV === "test";
var vite_config_default = defineConfig({
  plugins: [react(), !isTest && TanStackRouterVite()],
  resolve: {
    alias: {
      "@": path.resolve(__vite_injected_original_dirname, "./src")
    }
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJDOlxcXFxVc2Vyc1xcXFxtaWthZVxcXFxEb2N1bWVudHNcXFxcR2l0SHViXFxcXEthZmthQXVjdGlvblxcXFxmcm9udGVuZC1yZWFjdFwiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiQzpcXFxcVXNlcnNcXFxcbWlrYWVcXFxcRG9jdW1lbnRzXFxcXEdpdEh1YlxcXFxLYWZrYUF1Y3Rpb25cXFxcZnJvbnRlbmQtcmVhY3RcXFxcdml0ZS5jb25maWcudHNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfaW1wb3J0X21ldGFfdXJsID0gXCJmaWxlOi8vL0M6L1VzZXJzL21pa2FlL0RvY3VtZW50cy9HaXRIdWIvS2Fma2FBdWN0aW9uL2Zyb250ZW5kLXJlYWN0L3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IHsgZGVmaW5lQ29uZmlnIH0gZnJvbSBcInZpdGVcIjtcbmltcG9ydCByZWFjdCBmcm9tIFwiQHZpdGVqcy9wbHVnaW4tcmVhY3Qtc3djXCI7XG5pbXBvcnQgcGF0aCBmcm9tIFwicGF0aFwiO1xuaW1wb3J0IHsgVGFuU3RhY2tSb3V0ZXJWaXRlIH0gZnJvbSBcIkB0YW5zdGFjay9yb3V0ZXItdml0ZS1wbHVnaW5cIjtcblxuLy8gdml0ZXN0IGF1dG9tYXRpY2FsbHkgc2V0cyBOT0RFX0VOViB0byAndGVzdCcgd2hlbiBydW5uaW5nIHRlc3RzXG5jb25zdCBpc1Rlc3QgPSBwcm9jZXNzLmVudi5OT0RFX0VOViA9PT0gXCJ0ZXN0XCI7XG5cbi8vIGh0dHBzOi8vdml0ZWpzLmRldi9jb25maWcvXG5leHBvcnQgZGVmYXVsdCBkZWZpbmVDb25maWcoe1xuICBwbHVnaW5zOiBbcmVhY3QoKSwgIWlzVGVzdCAmJiBUYW5TdGFja1JvdXRlclZpdGUoKV0sXG4gIHJlc29sdmU6IHtcbiAgICBhbGlhczoge1xuICAgICAgXCJAXCI6IHBhdGgucmVzb2x2ZShfX2Rpcm5hbWUsIFwiLi9zcmNcIiksXG4gICAgfSxcbiAgfSxcbn0pO1xuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUFpWCxTQUFTLG9CQUFvQjtBQUM5WSxPQUFPLFdBQVc7QUFDbEIsT0FBTyxVQUFVO0FBQ2pCLFNBQVMsMEJBQTBCO0FBSG5DLElBQU0sbUNBQW1DO0FBTXpDLElBQU0sU0FBUyxRQUFRLElBQUksYUFBYTtBQUd4QyxJQUFPLHNCQUFRLGFBQWE7QUFBQSxFQUMxQixTQUFTLENBQUMsTUFBTSxHQUFHLENBQUMsVUFBVSxtQkFBbUIsQ0FBQztBQUFBLEVBQ2xELFNBQVM7QUFBQSxJQUNQLE9BQU87QUFBQSxNQUNMLEtBQUssS0FBSyxRQUFRLGtDQUFXLE9BQU87QUFBQSxJQUN0QztBQUFBLEVBQ0Y7QUFDRixDQUFDOyIsCiAgIm5hbWVzIjogW10KfQo=
